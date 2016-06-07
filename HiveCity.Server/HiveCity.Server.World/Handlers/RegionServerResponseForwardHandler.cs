using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class RegionServerResponseForwardHandler : DefaultResponseHandler
    {
        public RegionServerResponseForwardHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Response; }
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
            if (message.Parameters.ContainsKey((byte)ClientParameterCode.PeerId))
            {
                var peerId = new Guid((byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);
                Log.DebugFormat("Looking for peerId {0}", peerId);

                SubServerClientPeer peer;
                Server.ConnectionCollection<WorldServerConnectionCollection>().Clients.TryGetValue(new Guid((byte[])message.Parameters[(byte)ClientParameterCode.PeerId]), out peer);
                if (peer != null)
                {
                    Log.DebugFormat("Found peer");
                    var response = message as PhotonResponse;

                    if (response != null)
                    {
                        peer.ClientData<WorldServerClientData>().ProxyServer.SendOperationResponse(new OperationResponse(response.Code, response.Parameters)
                        {
                            DebugMessage = response.DebugMessage,
                            ReturnCode = response.ReturnCode
                        }, new SendParameters());
                    }
                    else
                    {
                        peer.ClientData<WorldServerClientData>()
                            .ProxyServer.SendOperationResponse(new OperationResponse(message.Code, message.Parameters), new SendParameters());
                    }
                }
            }

            return true;
        }
    }
}
