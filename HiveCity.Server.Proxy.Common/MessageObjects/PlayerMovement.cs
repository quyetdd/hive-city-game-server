// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerMovement.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the PlayerMovement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Common.MessageObjects
{
    using System;

    [Serializable]
    public class PlayerMovement
    {
        public int ObjectId { get; set; }

        public int Facing { get; set; }

        public bool Walk { get; set; }

        public bool Crouch { get; set; }

        public bool Moving { get; set; }

        public bool Jump { get; set; }

        public float Forward { get; set; }

        public float Right { get; set; }
    }
}
