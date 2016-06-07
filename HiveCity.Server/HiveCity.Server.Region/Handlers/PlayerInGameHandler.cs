// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerInGameHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the PlayerInGameHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Handlers
{
    using System;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Application;
    using HiveCity.Server.Photon.Server;
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Region.Models;

    public class PlayerInGameHandler : PhotonServerHandler
    {
        public PlayerInGameHandler(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return (byte)ClientOperationCode.Region; }
        }

        public override int? SubCode
        {
            get { return (int)MessageSubCode.PlayerInGame; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            // Part of the server but not in game yet
            var peerId = new Guid((byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);
            var clients = Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
            var instance = clients[peerId].ClientData<CPlayerInstance>();

            instance.Spawn();
            instance.BroadcastUserInfo();
            Log.DebugFormat("Character in the Region server!");

            // Notify Guild (Company) Members that player logged in

            // Notify Friend List
            return true;
        }
    }
}
