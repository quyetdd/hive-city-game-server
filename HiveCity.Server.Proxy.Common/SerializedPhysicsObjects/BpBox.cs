// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BpBox.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the BpBox type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Common.SerializedPhysicsObjects
{
    using System;

    using HiveCity.Server.Proxy.Common.MessageObjects;

    [Serializable]
    public class BpBox
    {
        public BpBox()
        {
            this.Center = new PositionData();
            this.HalfExtents = new PositionData();
            this.Rotation = new PositionData();
            this.LocalScale = new PositionData();
        }

        public PositionData Center { get; set; }

        public PositionData HalfExtents { get; set; } // Range around the centre 0.5 in all 3 dirs

        public PositionData Rotation { get; set; }

        public PositionData LocalScale { get; set; }
    }
}
