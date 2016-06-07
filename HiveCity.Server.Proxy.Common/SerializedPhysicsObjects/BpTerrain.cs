// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BpTerrain.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the BpTerrain type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Common.SerializedPhysicsObjects
{
    using System;

    using HiveCity.Server.Proxy.Common.MessageObjects;

    [Serializable]
    public class BpTerrain
    {
        public BpTerrain()
        {
            this.Center = new PositionData();
            this.LocalScale = new PositionData();
            this.Rotation = new PositionData();
        }

        public PositionData Center { get; set; }

        public PositionData LocalScale { get; set; } // 2000 * 2000 - big the terrain

        public PositionData Rotation { get; set; }

        public int Height { get; set; } // 513

        public int Width { get; set; } // 513

        public float[] HeightData { get; set; } // 0-1

        public float HeighScale { get; set; } // 0-600 Default Unity Terrain
    }
}