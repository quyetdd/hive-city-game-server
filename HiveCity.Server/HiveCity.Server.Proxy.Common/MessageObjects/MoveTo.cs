// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoveTo.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the MoveTo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Common.MessageObjects
{
    using System;

    [Serializable]
    public class MoveTo
    {
        public MoveTo()
        {
            this.CurrentPosition = new PositionData();
            this.Destination = new PositionData();
        }

        public PositionData CurrentPosition { get; set; }

        public PositionData Destination { get; set; }

        public int Facing { get; set; }

        public float Speed { get; set; }

        public bool Moving { get; set; }

        public MoveDirection Direction { get; set; }
    }
}
