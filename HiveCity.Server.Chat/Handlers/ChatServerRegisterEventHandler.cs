// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatServerRegisterEventHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChatServerRegisterEventHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Chat.Handlers
{
    using System;
    using System.Linq;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Application;
    using HiveCity.Server.Photon.Client;
    using HiveCity.Server.Photon.Server;
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Sub.Common;
    using HiveCity.Server.Sub.Common.Data.ClientData;
    using HiveCity.Server.Sub.Common.Data.NHibernate;

    public class ChatServerRegisterEventHandler : PhotonServerHandler
    {
        private readonly SubServerClientPeer.Factory clientFactory;

        public ChatServerRegisterEventHandler(PhotonApplication application, SubServerClientPeer.Factory clientFactory)
            : base(application) 
        {
            this.clientFactory = clientFactory;
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte)ServerEventCode.CharacterRegister; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var characterId = Convert.ToInt32(message.Parameters[(byte)ClientParameterCode.CharacterId]);
            var peerId = new Guid((byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);

            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var character =
                            session.QueryOver<PlayerCharacter>()
                                .Where(w => w.Id == characterId)
                                .List()
                                .FirstOrDefault();

                        transaction.Commit();

                        if (character != null)
                        {
                            var clients = Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
                            clients.Add(peerId, this.clientFactory(peerId));

                            // Add character data to the list of client List for chat
                            clients[peerId].ClientData<CharacterData>().CharacterId = characterId;
                            clients[peerId].ClientData<CharacterData>().UserId = Convert.ToInt32(message.Parameters[(byte)ClientParameterCode.UserId]);
                            clients[peerId].ClientData<ChatPlayer>().CharacterName = character.Name;
                            clients[peerId].ClientData<ServerData>().ServerPeer = serverPeer;
                            Log.DebugFormat("Character in the chat server!");
                           
                            // Notify Guild (Company) Members that player logged in

                            // Notify Friend List
                        }
                        else
                        {
                            Log.FatalFormat("ChatServerRegisterEventHandler - Should never reach character not found in the database.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                throw;
            }

            return true;
        }
    }
}
