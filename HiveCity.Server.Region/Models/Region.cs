// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Region.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the Region type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using HiveCity.Server.Region.Models.Interfaces;

    public class Region : IPlayerListener
    {
        private readonly Dictionary<int, IObject> allObjects;
        private readonly Dictionary<int, IPlayer> allPlayers;

        public Region()
        {
            this.allObjects = new Dictionary<int, IObject>();
            this.allPlayers = new Dictionary<int, IPlayer>();
        }

        #region All Object Functions

        public Dictionary<int, IObject>.ValueCollection VisibleObjects
        {
            get { return this.allObjects.Values; }
        }

        public void AddObject(IObject obj)
        {
            if (this.allObjects.ContainsKey(obj.ObjectId))
                // Log tried to add the same object twice
                return;

            this.allObjects.Add(obj.ObjectId, obj);
        }

        public void RemoveObject(IObject obj)
        {
            this.allObjects.Remove(obj.ObjectId);
        }

        public void RemoveObject(IEnumerable<IObject> list)
        {
            foreach (var obj in list)
            {
                if (obj != null)
                {
                    this.allObjects.Remove(obj.ObjectId);
                }
            }
        }

        public IObject FindObject(int id)
        {
            if (this.allObjects.ContainsKey(id))
            {
                return this.allObjects[id];
            }
            return null;
        }
         
        public T FindObject<T>(T objType, int objId) where T : class,IObject
        {
            return FindObject(objId) as T;
        }

        public IObject[] AllObjectsArray()
        {
            return this.allObjects.Values.ToArray();
        }

        public void ForEachObject(Action<IObject> action)
        {
            foreach (var allObject in this.allObjects)
            {
                action(allObject.Value);
            }
        }

        #endregion

        #region All Player Functions 

        public void AddPlayer(IPlayer player)
        {
            var obj = player as IObject;
            if (obj != null)
            {
                this.allPlayers.Add(obj.ObjectId, player);
            }

            if (OnAddPlayer != null)
            {
                OnAddPlayer(player);
            }
        }

        // Notify player addition
        public event Action<IPlayer> OnAddPlayer;
        public event Action<IPlayer> OnRemovePlayer;

        public void RemovePlayer(IPlayer player)
        {
            var obj = player as IObject;
            if (obj != null)
            {
                this.allPlayers.Remove(obj.ObjectId);
            }
            if (OnRemovePlayer != null)
            {
                OnRemovePlayer(player);
            }
        }

        public Dictionary<int, IPlayer> AllPlayers
        {
            get { return this.allPlayers; }
        }

        public IPlayer[] AllPlayersArray()
        {
            return this.allPlayers.Values.ToArray();
        }

        public void ForEachPlayer(Action<IPlayer> action)
        {
            foreach (var player in this.allPlayers)
            {
                action(player.Value);
            }
        }

        public int NumPlayers { get { return this.allPlayers.Count; } }

        public IPlayer GetPlayer(string name)
        {
            throw new NotImplementedException();
        }

        public IPlayer GetPlayer(int id)
        {
            if (this.allPlayers.ContainsKey(id))
            {
                return this.allPlayers[id];
            }
            return null;
        }

        #endregion

        #region Visible Object Manipulation

        public void AddVisibleObject(IObject obj)
        {
            var player = obj as IPlayer;
            if (player != null)
            {
                if (GetPlayer(obj.ObjectId) == null)
                {
                    AddPlayer(player);
                }
            }

            // Get all visible with 2000 units (2 KM) -- move to more sensible
            var visibles = GetVisibleObjects(obj, 2000);

            foreach (var visible in visibles)
            {
                if(visible == null)
                    continue;

                visible.KnownList.AddKnownObject(obj);
                obj.KnownList.AddKnownObject(visible);
            }
        }

        public void RemoveVisibleObject(IObject obj)
        {
            if (obj != null)
                return;

            this.allObjects.Remove(obj.ObjectId);

            ForEachObject(a => a.KnownList.RemoveKnownObject(obj));

            // Leaving region
            obj.KnownList.RemoveAllKnownObjects();

            var player = obj as IPlayer;
            if (player != null)
            {
               RemovePlayer(player);
            }
        }

        public List<IPlayable> GetVisiblePlayable(IObject obj)
        {
            return this.allPlayers.Values.Where(
                a => a != null && a != obj && a is IPlayable && a is IObject && ((IObject) a).IsVisible)
                .Cast<IPlayable>()
                .ToList();
        }

        public List<IObject> GetVisibleObjects(IObject obj, float radius = 0, bool use3D = false)
        {
            // Get Everything in Region
            if (radius == 0)
                return this.allObjects.Values.Where(a => a != null && a != obj && a.IsVisible).ToList();

            float sqRadius = radius*radius;

            float x = obj.Position.X;
            float y = obj.Position.Y;
            float z = obj.Position.Z;

            return this.allObjects.Values.Where(a =>
            {
                if (a == null || a == obj)
                {
                    return false;
                }
                float dx = a.Position.X - x;
                float dy = use3D ? a.Position.Y - y : 0;
                float dz = a.Position.Z - z;

                if (((dx*dx) + (dy*dy) + (dz*dz)) < sqRadius)
                {
                    return true;
                }
                return false;
            }).ToList();
        }

        #endregion
    }
}
