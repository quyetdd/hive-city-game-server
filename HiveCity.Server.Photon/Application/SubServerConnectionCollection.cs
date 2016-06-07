// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubServerConnectionCollection.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the SubServerConnectionCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Photon.Application
{
    using System;
    using System.Collections.Generic;

    using HiveCity.Server.Photon.Client;
    using HiveCity.Server.Photon.Server;

    using global::Photon.SocketServer;

    public class SubServerConnectionCollection : PhotonConnectionCollection
    {
        public SubServerConnectionCollection()
        {
            this.Clients = new Dictionary<Guid, SubServerClientPeer>();
        }

        public new Dictionary<Guid, SubServerClientPeer> Clients { get; protected set; }

        public override void Disconnect(PhotonServerPeer serverPeer)
        {
        }

        public override void Connect(PhotonServerPeer serverPeer)
        {
        }

        public override void ClientDisconnect(PhotonClientPeer clientPeer)
        {
            throw new NotImplementedException();
        }

        public override void ClientConnect(PhotonClientPeer clientPeer)
        {
            throw new NotImplementedException();
        }

        public void ClientConnect(SubServerClientPeer clientPeer)
        {
        }

        public void ClientDisconnect(SubServerClientPeer clientPeer)
        {
        }

        public override void ResetServers()
        {
        }

        public override bool IsServerPeer(InitRequest initRequest)
        {
            return false;
        }

        public override PhotonServerPeer OnGetServerByType(int serverType, params object[] additional)
        {
            throw new NotImplementedException();
        }

        public override void DisconnectAll()
        {
            foreach (var photonServerPeer in this.Servers)
            {
                photonServerPeer.Value.Disconnect();
            }
        }
    }
}
