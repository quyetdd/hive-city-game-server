// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BpMesh.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the BpMesh type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Common.SerializedPhysicsObjects
{
    using System;
    using System.Collections.Generic;

    using HiveCity.Server.Proxy.Common.MessageObjects;

    [Serializable]
    public class BpMesh
    {
        public BpMesh()
        {
            this.Rotation = new PositionData();
            this.LocalScale = new PositionData();

            this.Center = new PositionData();
            this.Vertexes = new List<PositionData>();
            this.Triangles = new List<int>();
        }

        public PositionData Center { get; set; }
        
        public PositionData Rotation { get; set; }
        
        public PositionData LocalScale { get; set; }

        public int NumTris { get; set; } // no of triangles
        
        public int NumVerts { get; set; } // no of vertices
        
        public List<int> Triangles { get; set; }

        public List<PositionData> Vertexes { get; set; }
    }
}
