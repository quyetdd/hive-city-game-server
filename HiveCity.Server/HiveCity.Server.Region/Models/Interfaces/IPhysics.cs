// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPhysics.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPhysics type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.Interfaces
{
    using BEPUutilities;

    using HiveCity.Server.Proxy.Common.MessageObjects;

    public interface IPhysics
    {
        Position Position { get; set; }

        bool Dirty { get; set; }

        PlayerMovement Movement { get; set; }

        MoveDirection Direction { get; set; }

        float MoveSpeed { get; set; }

        Vector3 WalkDirection { get; set; }

        bool Moving { get; set; }
    }
}
