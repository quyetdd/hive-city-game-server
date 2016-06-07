using HiveCity.Server.Framework;
using HiveCity.Server.Photon.Application;
using HiveCity.Server.Photon.Server;
using HiveCity.Server.Proxy.Common;
using HiveCity.Server.Sub.Common;
using HiveCity.Server.World.Models;
using Photon.SocketServer;
using System;

namespace HiveCity.Server.World.Handlers
{
    public class WorldServerDeregisterEventHandler : PhotonServerHandler
    {
        public WorldServerDeregisterEventHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte) ServerEventCode.CharacterDeRegister; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var peerId = new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);

            var clients = Server.ConnectionCollection<WorldServerConnectionCollection>().Clients;
            clients[peerId].ClientData<WorldServerClientData>().RegionServer.SendEvent(new EventData(message.Code, message.Parameters), new SendParameters());

            return true;
        }
    }
}
