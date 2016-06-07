// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonClientPeer.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the PhotonClientPeer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Photon.Client
{
    using System;
    using System.Collections.Generic;

    using ExitGames.Logging;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Application;
    using HiveCity.Server.Photon.Server;

    using global::Photon.SocketServer;

    using PhotonHostRuntimeInterfaces;

    public class PhotonClientPeer : PeerBase
    {
        protected ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly Guid peerId;
        private readonly Dictionary<Type, IClientData> clientData = new Dictionary<Type, IClientData>();
        private readonly PhotonApplication server;
        private readonly PhotonClientHandlerList handlerList;

        public PhotonServerPeer CurrentServer { get; set; }

        #region Factory Method

        public delegate PhotonClientPeer Factory(IRpcProtocol protocol, IPhotonPeer photonPeer);

        #endregion

        public PhotonClientPeer(IEnumerable<IClientData> clientData, IRpcProtocol protocol, IPhotonPeer photonPeer, PhotonClientHandlerList handlerList, PhotonApplication application)
            : base(protocol, photonPeer)
        {
            this.peerId = Guid.NewGuid();
            this.handlerList = handlerList;
            this.server = application;

            foreach (var data in clientData)
            {
                this.clientData.Add(data.GetType(), data);
            }

            this.server.ConnectionCollection<PhotonConnectionCollection>().Clients.Add(this.peerId, this);
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            this.handlerList.HandleMessage(new PhotonRequest(operationRequest.OperationCode, operationRequest.Parameters.ContainsKey(this.server.SubCodeParameterKey) ? (int?)Convert.ToInt32((operationRequest.Parameters[this.server.SubCodeParameterKey])) : null, operationRequest.Parameters), this);
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            this.server.ConnectionCollection<PhotonConnectionCollection>().OnClientDisconnect(this);
            this.Log.DebugFormat("Client {0} Disconnected", this.peerId);
        }

        public Guid PeerId
        {
            get { return this.peerId; }
        }

        // Get Specific Client data out
        public T ClientData<T>() where T : class, IClientData
        {
            IClientData result;

            this.clientData.TryGetValue(typeof (T), out result);
            if (result != null)
            {
                return result as T;
            }
            return null;
        }
    }
}