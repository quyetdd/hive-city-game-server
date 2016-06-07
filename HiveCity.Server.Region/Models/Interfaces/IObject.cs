// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IObject.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the IObject type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.Interfaces
{
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Region.Models.KnownList;
    using HiveCity.Server.Region.Models.ServerEvents;

    public interface IObject
    {
        int InstanceId { get; set; } // Unique

        int ObjectId { get; } // Db Id

        bool IsVisible { get; set; }

        string Name { get; set; }

        Position Position { get; set; }

        ObjectKnownList KnownList { get; set; }

        Region Region { get; }

        void Spawn(); // Create on client

        void Decay(); // Remove on client

        void SendPacket(ServerPacket packet);

        void SendPacket(SystemMessageId id);

        void SendInfo(IObject obj);

        void OnSpawn();

        void ToggleVisible();
    }
}
