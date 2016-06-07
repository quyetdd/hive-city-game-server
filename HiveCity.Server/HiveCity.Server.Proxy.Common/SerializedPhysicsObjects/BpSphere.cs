// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BpSphere.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the BpSphere type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Common.SerializedPhysicsObjects
{
    using System;

    using HiveCity.Server.Proxy.Common.MessageObjects;

    [Serializable]
    public class BpSphere
    {
        public BpSphere()
        {
            this.Rotation = new PositionData();
            this.LocalScale = new PositionData();
            this.Center = new PositionData();
        }

        public PositionData Center { get; set; }

        public PositionData Rotation { get; set; }

        public PositionData LocalScale { get; set; }

        public float Radius { get; set; }
    }
}
