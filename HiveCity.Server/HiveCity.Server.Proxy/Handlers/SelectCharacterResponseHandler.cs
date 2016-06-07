// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectCharacterResponseHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the SelectCharacterResponseHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Handlers
{
    using System;
    using System.Collections.Generic;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Application;
    using HiveCity.Server.Photon.Client;
    using HiveCity.Server.Photon.Server;
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Sub.Common;
    using HiveCity.Server.Sub.Common.Data.ClientData;

    using global::Photon.SocketServer;

    public class SelectCharacterResponseHandler : PhotonServerHandler
    {
        public SelectCharacterResponseHandler(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Response; }
        }

        public override byte Code
        {
            get { return (byte)ClientOperationCode.Login; }
        }

        public override int? SubCode
        {
            get { return (int)MessageSubCode.SelectCharacter; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            if (message.Parameters.ContainsKey((byte)ClientParameterCode.PeerId))
            {
                // ripping out the value in the dictionary and replacing with new guid
                var peerId = new Guid((byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);
                
                PhotonClientPeer peer;
                Server.ConnectionCollection<PhotonConnectionCollection>().Clients.TryGetValue(peerId, out peer);

                if (peer != null)
                {
                    int characterId = Convert.ToInt32(message.Parameters[(byte)ClientParameterCode.CharacterId]);
                    peer.ClientData<CharacterData>().CharacterId = characterId;

                    var para = new Dictionary<byte, object>()
                                   {
                                       {
                                           (byte)ClientParameterCode.CharacterId, characterId
                                       },
                                       {
                                           (byte)ClientParameterCode.PeerId, peerId.ToByteArray()
                                       },
                                       {
                                           (byte)ClientParameterCode.UserId, peer.ClientData<CharacterData>().UserId
                                       }
                                   };

                    var chatServer = Server.ConnectionCollection<ProxyConnectionCollection>().OnGetServerByType((int)ServerType.Chat);
                    if (chatServer != null)
                    {
                        chatServer.SendEvent(
                            new EventData((byte)ServerEventCode.CharacterRegister)
                                {
                                    Parameters = para
                                }, 
                            new SendParameters());
                    }

                    // TODO Add code to send same event to Region Server
                    var worldServer = Server.ConnectionCollection<ProxyConnectionCollection>().OnGetServerByType((int)ServerType.World);
                    if (worldServer != null)
                    {
                        Log.DebugFormat("Calling from SelectCharacterResponseHandler");
                        worldServer.SendEvent(
                            new EventData((byte)ServerEventCode.CharacterRegister)
                                {
                                    Parameters = para
                                },
                            new SendParameters());
                    }

                    message.Parameters.Remove((byte) ClientParameterCode.PeerId);
                    message.Parameters.Remove((byte) ClientParameterCode.UserId);
                    message.Parameters.Remove((byte) ClientParameterCode.CharacterId);

                    var response = message as PhotonResponse;
                    if (response != null)
                    {
                        peer.SendOperationResponse(
                            new OperationResponse(response.Code, response.Parameters)
                                {
                                    DebugMessage = response.DebugMessage,
                                    ReturnCode = response.ReturnCode
                                }, 
                            new SendParameters());
                    }
                    else
                    {
                        peer.SendOperationResponse(
                            new OperationResponse(message.Code, message.Parameters),
                            new SendParameters());
                    }
                }
            }

            return true;
        }
    }
}