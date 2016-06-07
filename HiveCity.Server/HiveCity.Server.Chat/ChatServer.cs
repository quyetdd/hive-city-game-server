using Autofac;
using HiveCity.Server.Framework;
using HiveCity.Server.Photon.Application;
using HiveCity.Server.Photon.Client;
using HiveCity.Server.Photon.Server;
using HiveCity.Server.Proxy.Common;
using HiveCity.Server.Sub.Common;
using HiveCity.Server.Sub.Common.Data;
using HiveCity.Server.Sub.Common.Data.ClientData;
using HiveCity.Server.Sub.Common.Handlers;
using HiveCity.Server.Sub.Common.Operations;
using Photon.SocketServer;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml.Serialization;

namespace HiveCity.Server.Chat
{
    public class ChatServer : PhotonApplication
    {
        // Local machine update for cluster servers
        // Ip Address proxy knows login is connected on
        private readonly IPAddress _publicIpAddress = IPAddress.Parse("127.0.0.1");
        private readonly IPEndPoint _masterEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4520);

        public override byte SubCodeParameterKey
        {
            get { return (byte)ClientParameterCode.SubOperationCode; }
        }

        public override IPEndPoint MasterEndPoint
        {
            get { return _masterEndPoint; }
        }

        public override int? TcpPort
        {
            get { return 4532; }
        }

        public override int? UdpPort
        {
            get { return 5057; }
        }

        public override IPAddress PublicIpAddress
        {
            get { return _publicIpAddress; }
        }

        public override int ServerType
        {
            get { return (int)Proxy.Common.ServerType.Chat; }
        }

        protected override int ConnectRetryIntervalSeconds
        {
            get { return 14; }
        }

        protected override bool ConnectsToMaster
        {
            get { return true; }
        }

        public override void Register(PhotonServerPeer peer)
        {
            var registerSubServerOperation = new RegisterSubServerData()
            {
                GameServerAddress = PublicIpAddress.ToString(),
                TcpPort = TcpPort,
                UdpPort = UdpPort,
                ServerId = ServerId,
                ServerType = ServerType,
                ApplicationName = ApplicationName
            };

            // Serialize our data
            var mySerializer = new XmlSerializer(typeof(RegisterSubServerData));
            var outString = new StringWriter();
            mySerializer.Serialize(outString, registerSubServerOperation);

            peer.SendOperationRequest(new OperationRequest((byte)ServerOperationCode.RegisterSubServer,
                new RegisterSubServer()
                {
                    RegisterSubServerOperation = outString.ToString()
                }), new SendParameters());
        }

        protected override void ResolveParameters(IContainer container)
        {
        }

        protected override void RegisterContainerObjects(ContainerBuilder builder)
        {
            builder.RegisterType<ErrorEventForwardHandler>().As<DefaultEventHandler>().SingleInstance();
            builder.RegisterType<ErrorRequestForwardHandler>().As<DefaultRequestHandler>().SingleInstance();
            builder.RegisterType<ErrorResponseForwardHandler>().As<DefaultResponseHandler>().SingleInstance();
            builder.RegisterType<SubServerConnectionCollection>().As<PhotonConnectionCollection>().SingleInstance();
            builder.RegisterInstance(this).As<PhotonApplication>().SingleInstance();
            builder.RegisterType<SubServerClientPeer>();
            builder.RegisterType<CharacterData>().As<IClientData>();
            builder.RegisterType<ChatPlayer>().As<IClientData>();
            builder.RegisterType<ServerData>().As<IClientData>();
            
            // Add Handlers
            builder.RegisterAssemblyTypes(Assembly.GetAssembly(GetType())).Where(w => w.Name.EndsWith("Handler")).As<PhotonServerHandler>().SingleInstance();
        }
    }
}
