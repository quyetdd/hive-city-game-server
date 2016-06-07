// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProxyServer.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Go between Servers and Client
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy
{
    using System;
    using System.Net;

    using Autofac;

    using HiveCity.Server.Photon.Application;
    using HiveCity.Server.Photon.Client;
    using HiveCity.Server.Photon.Server;
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Proxy.Handlers;
    using HiveCity.Server.Sub.Common.Data.ClientData;
    using HiveCity.Server.Sub.Common.Handlers;

    /// <summary>
    /// Go between Servers and Client
    /// </summary>
    public class ProxyServer : PhotonApplication
    {
        public override byte SubCodeParameterKey
        {
            get { return (byte)ClientParameterCode.SubOperationCode; }
        }

        /// <summary>
        /// This is the master(proxy)
        /// </summary>
        protected override bool ConnectsToMaster
        {
            get { return false; }
        }

        public override void Register(PhotonServerPeer peer)
        {
        }

        public override IPEndPoint MasterEndPoint
        {
            get { throw new NotImplementedException(); }
        }

        public override int? TcpPort
        {
            get { throw new NotImplementedException(); }
        }

        public override int? UdpPort
        {
            get { throw new NotImplementedException(); }
        }

        public override IPAddress PublicIpAddress
        {
            get { throw new NotImplementedException(); }
        }

        public override int ServerType
        {
            get { throw new NotImplementedException(); }
        }

        protected override int ConnectRetryIntervalSeconds
        {
            get { throw new NotImplementedException(); }
        }
        
        protected override void ResolveParameters(IContainer container)
        {
        }

        /// <summary>
        /// Register Types
        /// </summary>
        /// <param name="builder"></param>
        protected override void RegisterContainerObjects(ContainerBuilder builder)
        {
            builder.RegisterType<CharacterData>().As<Framework.IClientData>();
            builder.RegisterType<EventForwardHandler>().As<DefaultEventHandler>().SingleInstance();
            builder.RegisterType<ResponseForwardHandler>().As<DefaultResponseHandler>().SingleInstance();
            builder.RegisterType<RequestForwardHandler>().As<DefaultRequestHandler>().SingleInstance();
            builder.RegisterType<HandleServerRegistration>().As<PhotonServerHandler>().SingleInstance();
            builder.RegisterType<ProxyConnectionCollection>().As<PhotonConnectionCollection>().SingleInstance();
            builder.RegisterInstance(this).As<PhotonApplication>().SingleInstance();

            // Add handlers
            builder.RegisterType<HandleClientLoginRequests>().As<PhotonClientHandler>().SingleInstance();
            builder.RegisterType<HandleClientChatRequests>().As<PhotonClientHandler>().SingleInstance();
            builder.RegisterType<HandleClientRegionRequests>().As<PhotonClientHandler>().SingleInstance();
            builder.RegisterType<LoginResponseHandler>().As<PhotonServerHandler>().SingleInstance();
            builder.RegisterType<SelectCharacterResponseHandler>().As<PhotonServerHandler>().SingleInstance();
        }
    }
}
