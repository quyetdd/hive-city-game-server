// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhysicsBackgroundThread.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   60fps
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.BackgroundThreads
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Xml.Serialization;

    using BEPUphysics;
    using BEPUphysics.BroadPhaseEntries;
    using BEPUphysics.Character;
    using BEPUphysics.CollisionRuleManagement;
    using BEPUphysics.CollisionShapes;
    using BEPUphysics.Entities.Prefabs;

    using BEPUutilities;
    using BEPUutilities.Threading;

    using ExitGames.Logging;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Application;
    using HiveCity.Server.Proxy.Common.SerializedPhysicsObjects;
    using HiveCity.Server.Region.Models;
    using HiveCity.Server.Region.Models.Interfaces;
    using HiveCity.Server.Sub.Common;
    using HiveCity.Server.Sub.Common.Data.NHibernate;

    public class PhysicsBackgroundThread : IBackgroundThread
    {
        protected static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        protected PhotonApplication Server { get; set; }

        public Region Region { get; set; }

        public Space SpaceInstance { get; set; }

        private ParallelLooper parallelLooper;

        readonly CollisionGroup characters = new CollisionGroup();

        public float CharacterHeight = 1.75f;

        public float CharacterWidth = 0.75f;

        private bool isRunning;

        public PhysicsBackgroundThread(Region region, IEnumerable<IPlayerListener> playerListeners, PhotonApplication application)
        {
            this.Server = application;
            Region = region;

            Region.OnAddPlayer += this.OnAddPlayer;
            Region.OnRemovePlayer += this.OnRemovePlayer;

            foreach (var playerListener in playerListeners)
            {
                playerListener.OnAddPlayer += this.OnAddPlayer;
                playerListener.OnRemovePlayer += this.OnRemovePlayer;
            }
        }

        private void OnAddPlayer(IPlayer player)
        {
            // cast to object
            var obj = player as IObject;

            if (obj != null)
            {
                var cc =
                    ((BepuPhysics)player.Physics).CharacterController =
                    new CharacterController(
                        new Vector3(obj.Position.X, obj.Position.Y, obj.Position.Z),
                        this.CharacterHeight,
                        this.CharacterHeight / 2f,
                        this.CharacterHeight); // 10 = mass, stop slidding in friction

                // stop characters colliding
                cc.Body.CollisionInformation.CollisionRules.Group = this.characters;

                this.SpaceInstance.Add(cc);
            }
        }

        private void OnRemovePlayer(IPlayer player)
        {
            lock (this)
            {
                this.SpaceInstance.Remove(((BepuPhysics)player.Physics).CharacterController);
            }
        }

        public void Setup()
        {
            this.parallelLooper = new ParallelLooper();
            this.parallelLooper.AddThread();
            this.parallelLooper.AddThread();
            this.parallelLooper.AddThread();
            this.parallelLooper.AddThread();

            // 10 metres per sec grav
            // step calc at 30fps
            this.SpaceInstance = new Space(this.parallelLooper)
            {
                ForceUpdater = { Gravity = new Vector3(0, -10, 0) },
                TimeStepSettings = { TimeStepDuration = 1f / 30f }
            };

            // when a character collides with another character
            var groupPair = new CollisionGroupPair(this.characters, this.characters);
            CollisionRules.CollisionGroupRules.Add(groupPair, CollisionRule.NoBroadPhase); // characters pass through each other

            var filePath = Path.Combine(this.Server.BinaryPath, "default.xml");


            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var region = session.QueryOver<RegionRecord>()
                        .Where(w => w.Name == this.Server.ApplicationName)
                        .SingleOrDefault();

                    if (region != null)
                    {
                        filePath = Path.Combine(this.Server.BinaryPath, region.ColliderPath);
                    }
                }
            }

            var fileStream = File.OpenRead(filePath);
            var serializer = new XmlSerializer(typeof(BpColliders));
            var colliders = (BpColliders)serializer.Deserialize(fileStream);

            // Box col
            foreach (var bepuBox in colliders.Boxes)
            {
                var groundShape = new Box(
                    (Vector3)(Position)bepuBox.Center,
                    bepuBox.LocalScale.X * bepuBox.HalfExtents.X * 2,
                    bepuBox.LocalScale.Y * bepuBox.HalfExtents.Y * 2,
                    bepuBox.LocalScale.Z * bepuBox.HalfExtents.Z * 2)
                                      {
                                          Orientation =
                                              new Quaternion(
                                              bepuBox.Rotation.X,
                                              bepuBox.Rotation.Y,
                                              bepuBox.Rotation.Z,
                                              bepuBox.Rotation.W),
                                          IsAffectedByGravity = false
                                      };

                // stop falling
                this.SpaceInstance.Add(groundShape);
            }

            // Capsule col
            foreach (var bepuCapsule in colliders.Capsules)
            {
                var groundShape = new Capsule((Vector3)(Position)bepuCapsule.Center, bepuCapsule.LocalScale.Y * bepuCapsule.Height, bepuCapsule.LocalScale.Z * bepuCapsule.Radius)
                                      {
                                          Orientation = new Quaternion(bepuCapsule.Rotation.X, bepuCapsule.Rotation.Y, bepuCapsule.Rotation.Z, bepuCapsule.Rotation.W),
                                          IsAffectedByGravity = false
                                      };

                // stop falling
                this.SpaceInstance.Add(groundShape);
            }

            // Sphere
            foreach (var bepuSphere in colliders.Spheres)
            {
                var groundShape = new Sphere((Vector3)(Position)bepuSphere.Center, bepuSphere.LocalScale.X * bepuSphere.Radius)
                                      {
                                          Orientation = new Quaternion(bepuSphere.Rotation.X, bepuSphere.Rotation.Y, bepuSphere.Rotation.Z, bepuSphere.Rotation.W),
                                          IsAffectedByGravity = false
                                      };

                // stop falling
                this.SpaceInstance.Add(groundShape);
            }

            // Terrain col
            foreach (var bepuTerrain in colliders.Terrains)
            {
                var data = new float[bepuTerrain.Width, bepuTerrain.Height];
                for (int y = 0; y < bepuTerrain.Height; y++)
                {
                    for (int x = 0; x < bepuTerrain.Width; x++)
                    {
                        // convert to double array
                        data[x, y] = bepuTerrain.HeightData[(y * bepuTerrain.Width) + x];
                    }
                }

                var groundShape = new Terrain(
                    data,
                    new AffineTransform(
                        (Vector3)(Position)bepuTerrain.LocalScale,
                        new Quaternion(
                            bepuTerrain.Rotation.X,
                            bepuTerrain.Rotation.Y,
                            bepuTerrain.Rotation.Z,
                            bepuTerrain.Rotation.W),
                        (Vector3)(Position)bepuTerrain.Center))
                                      {
                                          Shape =
                                              {
                                                  QuadTriangleOrganization =
                                                      QuadTriangleOrganization
                                                      .BottomRightUpperLeft
                                              }
                                      };

                this.SpaceInstance.Add(groundShape);
            }

            // Mesh col
            foreach (var bepuMesh in colliders.Meshes)
            {
                var vectorList = new List<Vector3>();

                foreach (var data in bepuMesh.Vertexes)
                {
                    vectorList.Add(new Vector3(data.X, data.Y, data.Z));
                }

                var groundShape = new StaticMesh(
                    vectorList.ToArray(),
                    bepuMesh.Triangles.ToArray(),
                    new AffineTransform((Vector3)(Position)bepuMesh.LocalScale, new Quaternion(bepuMesh.Rotation.X, bepuMesh.Rotation.Y, bepuMesh.Rotation.Z, bepuMesh.Rotation.W), (Vector3)(Position)bepuMesh.Center));

                this.SpaceInstance.Add(groundShape);
            }

            fileStream.Close();
        }

        public void Run(object threadContext)
        {
            var timer = new Stopwatch();

            timer.Start();
            this.isRunning = true;

            // Stop the thread consuming if no players in region
            while (this.isRunning)
            {
                // Every 30 fps
                if (timer.Elapsed < TimeSpan.FromSeconds(1 / 30f))
                {
                    // if no players go to sleep for 1 second
                    if (Region.NumPlayers <= 0)
                    {
                        Thread.Sleep(1000);
                        timer.Restart();
                    }

                    if ((int)((1000f / 30f) - timer.Elapsed.Milliseconds) > 0)
                    {
                        Thread.Sleep((int)(1000f / 30f) - timer.Elapsed.Milliseconds);
                    }

                    continue;
                }

                var updateTime = timer.Elapsed;
                timer.Restart();

                this.Update(updateTime);
            }
        }

        private void Update(TimeSpan elapsed)
        {
            lock (this)
            {
                // expects to be in seconds
                this.SpaceInstance.Update();
            }

        }

        public void Stop()
        {
            this.isRunning = false;
        }
    }
}
