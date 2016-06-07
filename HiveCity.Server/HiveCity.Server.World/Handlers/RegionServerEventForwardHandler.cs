using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveCity.Server.Framework;
using HiveCity.Server.Photon.Application;
using HiveCity.Server.Photon.Client;
using HiveCity.Server.Photon.Server;
using HiveCity.Server.Proxy.Common;
using HiveCity.Server.World.Models;
using Photon.SocketServer;

namespace HiveCity.Server.World.Handlers
{
    public class RegionServerEventForwardHandler : DefaultEventHandler
    {
        public RegionServerEventForwardHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte) ClientOperationCode.Region; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            if (message.Parameters.ContainsKey((byte) ClientParameterCode.PeerId))
            {
                SubServerClientPeer peer;
                Server.ConnectionCollection<WorldServerConnectionCollection>().Clients.TryGetValue(new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId]), out peer);
                if (peer != null)
                {
                    peer.ClientData<WorldServerClientData>()
                        .ProxyServer.SendEvent(new EventData(message.Code, message.Parameters), new SendParameters());
                }
            }

            return true;
        }
    }
}
