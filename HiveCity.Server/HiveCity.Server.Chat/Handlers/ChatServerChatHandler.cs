// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatServerChatHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChatServerChatHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Chat.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Application;
    using HiveCity.Server.Photon.Server;
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Proxy.Common.MessageObjects;
    using HiveCity.Server.Sub.Common.Data.ClientData;

    using global::Photon.SocketServer;

    public class ChatServerChatHandler : PhotonServerHandler
    {
        public ChatServerChatHandler(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return (byte)ClientOperationCode.Chat; }
        }

        public override int? SubCode
        {
            get { return (byte)MessageSubCode.Chat; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            if (message.Parameters.ContainsKey((byte) ClientParameterCode.Object))
            {
                var peerId = new Guid((byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);

                var mySerializer = new XmlSerializer(typeof(ChatItem));
                var inStream = new StringReader((string)message.Parameters[(byte)ClientParameterCode.Object]);

                var chatItem = (ChatItem)mySerializer.Deserialize(inStream);
         
                switch (chatItem.Type)
                {
                    case ChatType.Local:
                    case ChatType.Region:
                    case ChatType.Company:
                    case ChatType.Squad:
                    case ChatType.Trade:
                    case ChatType.PrivateMessage:
                    case ChatType.General:
                        chatItem.Text = string.Format("[General] {0}: {1}", Server.ConnectionCollection<SubServerConnectionCollection>().Clients[peerId].ClientData<ChatPlayer>().CharacterName, chatItem.Text);
                        var outStream = new StringWriter();
                        mySerializer.Serialize(outStream, chatItem);
                        foreach (var client in Server.ConnectionCollection<SubServerConnectionCollection>().Clients)
                        {
                            var para = new Dictionary<byte, object>()
                            {
                                {(byte)ClientParameterCode.PeerId, client.Key.ToString()},
                                {(byte)ClientParameterCode.Object, outStream.ToString()},
                            };
                            var eventData = new EventData {Code = (byte)ClientEventCode.Chat, Parameters = para};
                            
                            // if behind multiple proxies
                            client.Value.ClientData<ServerData>().ServerPeer.SendEvent(eventData, new SendParameters());
                        }
                        break;
                }
            }

            return true;
        }
    }
}
