// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegionServerRegisterEventHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the RegionServerRegisterEventHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Handlers
{
    using System;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Application;
    using HiveCity.Server.Photon.Client;
    using HiveCity.Server.Photon.Server;
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Region.Models;
    using HiveCity.Server.Sub.Common;

    public class RegionServerRegisterEventHandler : PhotonServerHandler
    {
        private readonly SubServerClientPeer.Factory clientFactory;

        public RegionServerRegisterEventHandler(PhotonApplication application, SubServerClientPeer.Factory clientFactory)
            : base(application)
        {
            this.clientFactory = clientFactory;
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte)ServerEventCode.CharacterRegister; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            int characterId = Convert.ToInt32(message.Parameters[(byte)ClientParameterCode.CharacterId]);
            var peerId = new Guid((byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);

            try
            {
                var clients = Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
                clients.Add(peerId, this.clientFactory(peerId));

                // Add character data to the list of client List for chat
                var instance = clients[peerId].ClientData<CPlayerInstance>();

                instance.UserId = Convert.ToInt32(message.Parameters[(byte)ClientParameterCode.UserId]);
                instance.ServerPeer = serverPeer;
                instance.Client = clients[peerId];
      
                // Load character
                instance.Restore(characterId);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                throw;
            }

            return true;
        }
    }
}
