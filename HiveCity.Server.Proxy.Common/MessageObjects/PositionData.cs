// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PositionData.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the PositionData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Common.MessageObjects
{
    using System;

    [Serializable]
    public class PositionData
    {
        public PositionData()
            : this(0, 0, 0, 0)
        {
        }

        public PositionData(float x, float y, float z)
            : this(x, y, z, 0)
        {
        }

        public PositionData(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
            this.Heading = 0;
        }

        public PositionData(float x, float y, float z, short heading)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = 0;
            this.Heading = heading;
        }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float W { get; set; }

        public short Heading { get; set; } // 0 - 65535  0.00549 deg per int value (shorten mem space)
    }
}
