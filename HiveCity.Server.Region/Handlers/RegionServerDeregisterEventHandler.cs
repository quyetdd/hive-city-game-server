// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegionServerDeregisterEventHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the RegionServerDeregisterEventHandler type.
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
    using HiveCity.Server.Sub.Common;

    public class RegionServerDeregisterEventHandler : PhotonServerHandler
    {
        public RegionServerDeregisterEventHandler(PhotonApplication application) : base(application)
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
            // ToDo: broke this 
            var peerId = new Guid((byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);

            // Remove from groups (group disconnect - time to relogin), companies, etc.
            var instance = Server.ConnectionCollection<SubServerConnectionCollection>().Clients[peerId].ClientData<CPlayerInstance>();
            instance.DeleteMe();

            Server.ConnectionCollection<SubServerConnectionCollection>().Clients.Remove(peerId);
    
            Log.DebugFormat("Removed peer {0}, now we have {1} clients.", peerId, Server.ConnectionCollection<SubServerConnectionCollection>().Clients.Count);
            
            return true;
        }
    }
}