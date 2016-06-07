// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPlayer.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPlayer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.Interfaces
{
    using HiveCity.Server.Photon.Client;
    using HiveCity.Server.Photon.Server;

    public interface IPlayer
    {
        SubServerClientPeer Client { get; set; }

        PhotonServerPeer ServerPeer { get; set; }

        int? UserId { get; set; }

        int? CharacterId { get; set; }

        // TODO - Physics
        IPhysics Physics { get; set; }
    }
}
