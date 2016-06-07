// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonApplication.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the PhotonApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Photon.Application
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;

    using Autofac;

    using ExitGames.Logging;
    using ExitGames.Logging.Log4Net;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Client;
    using HiveCity.Server.Photon.Server;

    using log4net;
    using log4net.Config;

    using global::Photon.SocketServer;

    using global::Photon.SocketServer.ServerToServer;

    using LogManager = ExitGames.Logging.LogManager;

    public abstract class PhotonApplication : ApplicationBase
    {
        protected static readonly Guid ServerId = Guid.NewGuid();
        
        public abstract byte SubCodeParameterKey { get; }
        
        private PhotonConnectionCollection connectionCollection { get; set; }
        
        // Master server
        public abstract IPEndPoint MasterEndPoint { get; }

        public abstract int? TcpPort { get; }

        public abstract int? UdpPort { get; }

        public abstract IPAddress PublicIpAddress { get; }

        public abstract int ServerType { get; }

        protected abstract int ConnectRetryIntervalSeconds { get; }
        protected abstract bool ConnectsToMaster { get; }

        private static PhotonServerPeer _masterPeer;
        private byte _isReconnecting;
        private Timer _retry;

        private PhotonPeerFactory _factory;                             // Receive connections, how to create peer.
        private IEnumerable<IBackgroundThread> _backgroundThreads;      // Autofac filled automatically 


        public readonly ILogger Log = LogManager.GetCurrentClassLogger();

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return _factory.CreatePeer(initRequest);
        }

        protected override void Setup()
        {
            LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
            // Change Logging name for each server (diff log names)
            GlobalContext.Properties["LogFileName"] = ApplicationName;
            // Update automatically if Log4Net changes
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(BinaryPath, "log4net.config")));

            var builder = new ContainerBuilder();

            Initialize(builder);

            var container = builder.Build();

            // Any Peer factory added will be put into this factory
            _factory = container.Resolve<PhotonPeerFactory>();
            connectionCollection = container.Resolve<PhotonConnectionCollection>();
            _backgroundThreads = container.Resolve<IEnumerable<IBackgroundThread>>();

            // Resolve the container from inside whatever application
            ResolveParameters(container);

            foreach (var backgroundThread in _backgroundThreads)
            {
                backgroundThread.Setup();
                ThreadPool.QueueUserWorkItem(backgroundThread.Run);
            }

            if (ConnectsToMaster)
            {
                ConnectToMaster();
            }
        }

        public void ConnectToMaster()
        {
            if (!ConnectToServer(MasterEndPoint, "Proxy", "Proxy"))
            {
                Log.Warn("Proxy Connection refused");
                return;
            }
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat(_isReconnecting == 0 ? "Connect to Proxy at {0}" : "Reconnected to Proxy at {0}",
                    MasterEndPoint);
            }
        }

        protected void Initialize(ContainerBuilder builder)
        {
            builder.RegisterType<PhotonPeerFactory>();
            builder.RegisterType<PhotonServerPeer>();
            builder.RegisterType<PhotonClientPeer>();
            builder.RegisterType<PhotonClientHandlerList>();
            builder.RegisterType<PhotonServerHandlerList>();

            RegisterContainerObjects(builder);
        }

        protected override void TearDown()
        {
        }

        // When dll's are changed Photon will kick this function
        protected override void OnStopRequested()
        {
            // stop all threads
            foreach (var backgroundThread in _backgroundThreads)
            {
                backgroundThread.Stop();
            }

           if(connectionCollection != null)
               connectionCollection.DisconnectAll();

            base.OnStopRequested();
        }

        protected override void OnServerConnectionFailed(int errorCode, string errorMessage, object state)
        {
            if (_isReconnecting == 0)
            {
                Log.ErrorFormat("Proxy Connection failed with error {0} : {1}", errorCode, errorMessage);
            }
            else if (Log.IsDebugEnabled)
            {
                Log.ErrorFormat("Proxy Connection failed with error {0} : {1}", errorCode, errorMessage);
            }

            string stateString = state as string;

            if (stateString != null && stateString.Equals("Proxy"))
            {
                ReconnectToMaster();
            }

            base.OnServerConnectionFailed(errorCode, errorMessage, state);
        }

        public void ReconnectToMaster()
        {
            // Now Reconnect
            Thread.VolatileWrite(ref _isReconnecting, 1);

            _retry = new Timer(o => ConnectToMaster(), null, ConnectRetryIntervalSeconds * 1000, 0);
        }

        protected override ServerPeerBase CreateServerPeer(InitResponse initResponse, object state)
        {
            // return server created
            return _factory.CreatePeer(initResponse);
        }

        public T BackgroundThread<T>() where T : class
        {
            IBackgroundThread result;
            result = _backgroundThreads.ToList().Find(s => s.GetType() == typeof (T));
            if (result != null)
            {
                return result as T;
            }

            return null;
        }

        public TConnectionCollection ConnectionCollection<TConnectionCollection>() 
            where TConnectionCollection : PhotonConnectionCollection
        {
            return connectionCollection as TConnectionCollection;
        }

        // Allow to pass in types from applications
        protected abstract void RegisterContainerObjects(ContainerBuilder builder);
        protected abstract void ResolveParameters(IContainer container);
        public abstract void Register(PhotonServerPeer peer);
    }
}
