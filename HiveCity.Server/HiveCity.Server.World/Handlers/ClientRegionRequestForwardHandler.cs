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
    public class ClientRegionRequestForwardHandler : DefaultRequestHandler
    {
        public ClientRegionRequestForwardHandler(PhotonApplication application)
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
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Log.DebugFormat("Here we are, forwarding a client message to the region server");
            var operationRequest = new OperationRequest(message.Code, message.Parameters);
            var peerId = new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);

            Log.DebugFormat("Looking for peerId {0}", peerId);
            SubServerClientPeer peer;
            Server.ConnectionCollection<WorldServerConnectionCollection>().Clients.TryGetValue(peerId, out peer);
            if (peer != null)
            {
                peer.ClientData<WorldServerClientData>().RegionServer.SendOperationRequest(operationRequest, new SendParameters());
            }
            return true;
        }
    }
}
