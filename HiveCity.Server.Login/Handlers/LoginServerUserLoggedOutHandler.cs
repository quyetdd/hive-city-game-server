using System;
using HiveCity.Server.Framework;
using HiveCity.Server.Photon.Application;
using HiveCity.Server.Photon.Server;
using HiveCity.Server.Proxy.Common;
using HiveCity.Server.Sub.Common;

namespace HiveCity.Server.Login.Handlers
{
    public class LoginServerUserLoggedOutHandler : PhotonServerHandler
    {
        public LoginServerUserLoggedOutHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte) ServerEventCode.UserLoggedOut; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var server = Server as LoginServer;
            var peerId = new Guid((byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);

            if (server != null)
            {
               // var peerId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]);
                server.ConnectionCollection<SubServerConnectionCollection>().Clients.Remove(peerId);
            }

            return true;
        }
    }
}
