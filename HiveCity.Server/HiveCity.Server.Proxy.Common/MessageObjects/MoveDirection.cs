// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoveDirection.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the MoveDirection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Common.MessageObjects
{
    using System;

    [Flags]
    public enum MoveDirection
    {
        None = 0,
        Forward = 1,
        Backward = 2,
        Left = 4,
        Right = 8,
        ForwardLeft = 5,
        ForwardRight = 9,
        BackwardLeft = 6,
        BackwardRight = 10
    }
}
