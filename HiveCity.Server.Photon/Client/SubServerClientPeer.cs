// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubServerClientPeer.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the SubServerClientPeer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Photon.Client
{
    using System;
    using System.Collections.Generic;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Application;

    public class SubServerClientPeer : IClient
    {
        private readonly Guid peerId;
        private readonly Dictionary<Type, IClientData> clientData = new Dictionary<Type, IClientData>();
        private readonly PhotonApplication server;

        public Guid PeerId
        {
            get
            {
                return this.peerId;
            }
        }

        public PhotonApplication Server
        {
            get
            {
                return this.server;
            }
        }

        // Inject into constructor
        public delegate SubServerClientPeer Factory(Guid peerId);

        public SubServerClientPeer(IEnumerable<IClientData> clientData, PhotonApplication application)
        {
            this.peerId = Guid.NewGuid();
            this.server = application;
            foreach (var data in clientData)
            {
                this.clientData.Add(data.GetType(), data);
            }
        }

        public T ClientData<T>() where T : class, IClientData
        {
            Framework.IClientData result;
            this.clientData.TryGetValue(typeof(T), out result);
            if (result != null)
            {
                return result as T;
            }
            return null;
        }
    }
}
