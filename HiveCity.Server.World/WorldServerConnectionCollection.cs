using System;
using System.Collections.Generic;
using System.Linq;
using HiveCity.Server.Photon.Application;
using HiveCity.Server.Photon.Client;
using HiveCity.Server.Photon.Server;
using HiveCity.Server.Proxy.Common;
using HiveCity.Server.Proxy.Common.MessageObjects;
using HiveCity.Server.Region.Models;
using HiveCity.Server.World.Models;
using Photon.SocketServer;

namespace HiveCity.Server.World
{
    public class WorldServerConnectionCollection : PhotonConnectionCollection
    {
        public new Dictionary<Guid, SubServerClientPeer> Clients { get; protected set; }

        public WorldServerConnectionCollection()
        {
            Clients = new Dictionary<Guid, SubServerClientPeer>();
        }

        /// <summary>
        /// Subservers connecting
        /// </summary>
        /// <param name="serverPeer"></param>
        public override void Connect(PhotonServerPeer serverPeer)
        {
            if ((serverPeer.ServerType & (int)ServerType.Region) != 0)
                Log.DebugFormat("Got an incoming Region Server");

        }

        public override void Disconnect(PhotonServerPeer serverPeer)
        {
        }

        /// <summary>
        /// No client peers disconnecting
        /// </summary>
        /// <param name="clientPeer"></param>
        public override void ClientDisconnect(PhotonClientPeer clientPeer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// No client peers connecting
        /// </summary>
        /// <param name="clientPeer"></param>
        public override void ClientConnect(PhotonClientPeer clientPeer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overrides the above
        /// </summary>
        /// <param name="clientPeer"></param>
        public void ClientConnect(SubServerClientPeer clientPeer)
        {
        }

        /// <summary>
        /// Overrides the above
        /// </summary>
        /// <param name="clientPeer"></param>
        public void ClientDisconnect(SubServerClientPeer clientPeer)
        {
        }

        public override void ResetServers()
        {
        }

        public override bool IsServerPeer(InitRequest initRequest)
        {
            Log.DebugFormat("Received init request to {0} : {1} - {2}", initRequest.LocalIP, initRequest.LocalPort, initRequest);
            if (initRequest.LocalPort == 4533)
                return true;

            return false;
        }

        public override PhotonServerPeer OnGetServerByType(int serverType, params object[] additional)
        {
            if (serverType == (int) ServerType.Region)
            {
                foreach (PhotonServerPeer photonServerPeer in Servers.Values.Where(w => w.ServerType == (int)ServerType.Region))
                {
                    //if (additional[0] is Position &&
                    //    photonServerPeer.ServerData<RegionInfo>().Contains(additional[0] as Position))
                    //{
                    //    return photonServerPeer;
                    //}
                    if (additional[0] is Position)
                    {
                        return photonServerPeer;
                    }

                }
            }
            return null;
        }

        public override void DisconnectAll()
        {
            throw new NotImplementedException();
        }
    }
}
