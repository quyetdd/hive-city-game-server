// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BpColliders.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the BpColliders type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Common.SerializedPhysicsObjects
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class BpColliders
    {
        public BpColliders()
        {
            this.Boxes = new List<BpBox>();
            this.Capsules = new List<BpCapsule>();
            this.Meshes = new List<BpMesh>();
            this.Spheres = new List<BpSphere>();
            this.Terrains = new List<BpTerrain>();
        }

        public List<BpBox> Boxes { get; set; }

        public List<BpCapsule> Capsules { get; set; }

        public List<BpMesh> Meshes { get; set; }

        public List<BpSphere> Spheres { get; set; }

        public List<BpTerrain> Terrains { get; set; }
    }
}