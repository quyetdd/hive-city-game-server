using HiveCity.Server.Framework;
using HiveCity.Server.Photon.Application;
using HiveCity.Server.Photon.Client;
using HiveCity.Server.Photon.Server;
using HiveCity.Server.Proxy.Common;
using HiveCity.Server.Region.Models;
using HiveCity.Server.Sub.Common;
using HiveCity.Server.Sub.Common.Data.NHibernate;
using HiveCity.Server.World.Models;
using Photon.SocketServer;
using System;

namespace HiveCity.Server.World.Handlers
{
    public class WorldServerRegisterEventHandler : PhotonServerHandler
    {
        private readonly SubServerClientPeer.Factory _clientFactory;

        public WorldServerRegisterEventHandler(PhotonApplication application, SubServerClientPeer.Factory clientFactory) : base(application)
        {
            _clientFactory = clientFactory;
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte) ServerEventCode.CharacterRegister; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            int characterId = Convert.ToInt32(message.Parameters[(byte) ClientParameterCode.CharacterId]);
            var peerId = new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);

            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var character = session.QueryOver<PlayerCharacter>().Where(w => w.Id == characterId).SingleOrDefault();
                        if (character != null)
                        {
                            transaction.Commit();
                            var clients = Server.ConnectionCollection<WorldServerConnectionCollection>().Clients;
                            clients.Add(peerId, _clientFactory(peerId));

                            // TODO: Deserialise character.position
                            var server =
                                Server.ConnectionCollection<WorldServerConnectionCollection>()
                                    .GetServerByType(8, new Position(0, 0, 0)); // character.Position

                            server.SendEvent(new EventData(message.Code, message.Parameters), new SendParameters());
                            clients[peerId].ClientData<WorldServerClientData>().RegionServer = server;
                            clients[peerId].ClientData<WorldServerClientData>().ProxyServer = serverPeer;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("An Error Occured :- {0}", e);
                throw;
            }

            return true;
        }
    }
}
