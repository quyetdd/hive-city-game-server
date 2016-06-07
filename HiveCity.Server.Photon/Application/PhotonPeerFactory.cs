// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonPeerFactory.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the PhotonPeerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Photon.Application
{
    using System;

    using ExitGames.Logging;

    using HiveCity.Server.Photon.Client;
    using HiveCity.Server.Photon.Server;

    using global::Photon.SocketServer;

    public class PhotonPeerFactory
    {
        protected readonly ILogger Log;

        private readonly PhotonServerPeer.Factory serverPeerFactory;
        private readonly PhotonClientPeer.Factory clientPeerFactory;
        private readonly PhotonConnectionCollection subServerConnection;
        private readonly PhotonApplication application;

        public PhotonPeerFactory(
            PhotonServerPeer.Factory serverPeerFactory,
            PhotonClientPeer.Factory clientPeerFactory, 
            PhotonConnectionCollection subServerConnection, 
            PhotonApplication application)
        {        
            this.serverPeerFactory = serverPeerFactory;
            this.clientPeerFactory = clientPeerFactory;
            this.subServerConnection = subServerConnection;
            this.application = application;
            this.Log = this.application.Log;
        }

        /// <summary>
        /// Client or Sub-server could connect through.
        /// </summary>
        public PeerBase CreatePeer(InitRequest initRequest)
        {
            if (this.IsServerPeer(initRequest))
            {
                if (this.Log.IsDebugEnabled)
                {
                    this.Log.DebugFormat("Received Init Request from sub server");
                }

                return this.serverPeerFactory(initRequest.Protocol, initRequest.PhotonPeer);
            }

            this.Log.DebugFormat("Received Init Request from Client");

            return this.clientPeerFactory(initRequest.Protocol, initRequest.PhotonPeer);
        }

        /// <summary>
        /// When connecting to master server from Sub-server
        /// Sub-server trying to connect.
        /// </summary>
        public PhotonServerPeer CreatePeer(InitResponse initResponse)
        {
            try
            {
                var subServerPeer = this.serverPeerFactory(initResponse.Protocol, initResponse.PhotonPeer);
                if (this.Log.IsDebugEnabled)
                {
                    this.Log.DebugFormat("Received Init Request from Subserver");
                }

                if (initResponse.RemotePort == this.application.MasterEndPoint.Port)
                {
                    this.application.Register(subServerPeer);
                    if (this.Log.IsDebugEnabled)
                    {
                        this.Log.DebugFormat("Registering sub server");
                    }
                }

                return subServerPeer;
            }
            catch (Exception e)
            {
                this.Log.ErrorFormat(e.ToString());

                throw;
            }

            return null;
        }

        protected bool IsServerPeer(InitRequest initRequest)
        {
            return this.subServerConnection.IsServerPeer(initRequest);
        }
    }
}
