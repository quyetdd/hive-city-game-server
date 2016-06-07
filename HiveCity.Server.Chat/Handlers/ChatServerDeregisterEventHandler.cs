// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatServerDeregisterEventHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChatServerDeregisterEventHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Chat.Handlers
{
    using System;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Application;
    using HiveCity.Server.Photon.Server;
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Sub.Common;

    public class ChatServerDeregisterEventHandler : PhotonServerHandler
    {
        public ChatServerDeregisterEventHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte)ServerEventCode.CharacterDeRegister; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var peerId = new Guid((byte[])message.Parameters[(byte) ClientParameterCode.PeerId]);
            
            // Remove from groups (group disconnect - time to relogin), companies, etc.
            Server.ConnectionCollection<SubServerConnectionCollection>().Clients.Remove(peerId);
    
            Log.DebugFormat("Removed peer {0}, now we have {1} clients.", peerId, Server.ConnectionCollection<SubServerConnectionCollection>().Clients.Count);
            
            return true;
        }
    }
}
