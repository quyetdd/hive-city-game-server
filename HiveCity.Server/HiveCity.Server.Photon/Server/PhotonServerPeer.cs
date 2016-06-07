// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonServerPeer.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the PhotonServerPeer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Photon.Server
{
    using System;
    using System.Collections.Generic;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Application;

    using global::Photon.SocketServer;

    using global::Photon.SocketServer.ServerToServer;

    using PhotonHostRuntimeInterfaces;

    public class PhotonServerPeer : ServerPeerBase
    {
        private readonly PhotonServerHandlerList handlerList;
        protected readonly PhotonApplication Server;
        
        public Guid? ServerId;

        public string TcpAddress { get; set; }

        public string UdpAddress { get; set; }

        public string ApplicationName { get; set; }

        public int ServerType { get; set; }

        private readonly Dictionary<Type, IServerData> serverData = new Dictionary<Type, IServerData>(); 
        
        #region Factory Method

        // Send in specific per server connection
        public delegate PhotonServerPeer Factory(IRpcProtocol protocol, IPhotonPeer photonPeer);

        #endregion 

        public PhotonServerPeer(
            IRpcProtocol protocol, 
            IPhotonPeer photonPeer, 
            IEnumerable<IServerData> serverData, 
            PhotonServerHandlerList handlerList, 
            PhotonApplication application)
            : base(protocol, photonPeer)
        {
            this.handlerList = handlerList;
            this.Server = application;

            foreach (var data in serverData)
            {
              this.serverData.Add(data.GetType(), data);   
            }
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            this.handlerList.HandleMessage(new PhotonRequest(operationRequest.OperationCode, operationRequest.Parameters.ContainsKey(Server.SubCodeParameterKey) ? (int?)Convert.ToInt32((operationRequest.Parameters[Server.SubCodeParameterKey])) : null, operationRequest.Parameters), this);
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            this.Server.ConnectionCollection<PhotonConnectionCollection>().OnDisconnect(this);
        }

        protected override void OnEvent(IEventData eventData, SendParameters sendParameters)
        {
            this.handlerList.HandleMessage(new PhotonEvent(eventData.Code, eventData.Parameters.ContainsKey(Server.SubCodeParameterKey) ? (int?)Convert.ToInt32(eventData.Parameters[this.Server.SubCodeParameterKey]) : null, eventData.Parameters), this);
        }

        protected override void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
        {
            this.handlerList.HandleMessage(new PhotonResponse(operationResponse.OperationCode, operationResponse.Parameters.ContainsKey(this.Server.SubCodeParameterKey) ? (int?)Convert.ToInt32(operationResponse.Parameters[this.Server.SubCodeParameterKey]) : null, operationResponse.Parameters, operationResponse.DebugMessage, operationResponse.ReturnCode), this);
        }

        public T ServerData<T>() where T : class, IServerData
        {
            IServerData result;
            this.serverData.TryGetValue(typeof(T), out result);
            if (result != null)
            {
                return result as T;
            }

            return null;
        }
    }
}
