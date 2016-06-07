// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonConnectionCollection.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the PhotonConnectionCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Photon.Application
{
    using System;
    using System.Collections.Generic;

    using ExitGames.Logging;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Client;
    using HiveCity.Server.Photon.Server;

    using global::Photon.SocketServer;

    public abstract class PhotonConnectionCollection : IConnectionCollection<PhotonServerPeer, PhotonClientPeer>
    {
        protected readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public Dictionary<Guid, PhotonClientPeer> Clients { get; protected set; }

        public Dictionary<Guid, PhotonServerPeer> Servers { get; protected set; }

        protected PhotonConnectionCollection()
        {
            this.Servers = new Dictionary<Guid, PhotonServerPeer>();
            this.Clients = new Dictionary<Guid, PhotonClientPeer>();
        }

        public void OnConnect(PhotonServerPeer serverPeer)
        {
            if (!serverPeer.ServerId.HasValue)
            {
                throw new InvalidOperationException("Server Id cannot be null!");
            }

            var id = serverPeer.ServerId.Value;

            lock (this)
            {
                PhotonServerPeer peer;
                if (this.Servers.TryGetValue(id, out peer))
                {
                    peer.Disconnect();
                    this.Servers.Remove(id);
                    this.Disconnect(peer);
                }

                this.Servers.Add(id, serverPeer);

                Log.Warn("Sending to Connect");

                this.Connect(serverPeer);

                this.ResetServers();
            }
        }

        public void OnDisconnect(PhotonServerPeer serverPeer)
        {
            if (!serverPeer.ServerId.HasValue)
            {
                this.Disconnect(serverPeer);
                throw new InvalidOperationException("Server Id cannot be null");
            }

            lock (this)
            {
                PhotonServerPeer peer;

                // Lose a server - find a new one
                var id = serverPeer.ServerId.Value;
                if (!this.Servers.TryGetValue(id, out peer))
                {
                    return;
                }

                if (peer != serverPeer)
                {
                    return;
                }

                this.Servers.Remove(id);
                this.Disconnect(peer);
                this.ResetServers();
            }
        }

        public void OnClientConnect(PhotonClientPeer clientPeer)
        {
            this.ClientConnect(clientPeer);
            this.Clients.Add(clientPeer.PeerId, clientPeer);
        }

        public void OnClientDisconnect(PhotonClientPeer clientPeer)
        {
            this.ClientDisconnect(clientPeer);
            this.Clients.Remove(clientPeer.PeerId);
        }

        public PhotonServerPeer GetServerByType(int serverType, params object[] additional)
        {
            return this.OnGetServerByType(serverType, additional);
        }

        // Deferred for application specific
        public abstract void Disconnect(PhotonServerPeer serverPeer);

        public abstract void Connect(PhotonServerPeer serverPeer);

        public abstract void ClientDisconnect(PhotonClientPeer clientPeer);

        public abstract void ClientConnect(PhotonClientPeer clientPeer);

        public abstract void ResetServers();

        public abstract bool IsServerPeer(InitRequest initRequest);

        public abstract PhotonServerPeer OnGetServerByType(int serverType, params object[] additional);

        public abstract void DisconnectAll(); // Kick everyone off
    }
}
