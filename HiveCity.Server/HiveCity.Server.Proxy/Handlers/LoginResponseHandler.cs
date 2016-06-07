// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginResponseHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the LoginResponseHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Handlers
{
    using System;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Application;
    using HiveCity.Server.Photon.Client;
    using HiveCity.Server.Photon.Server;
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Sub.Common.Data.ClientData;

    using global::Photon.SocketServer;

    public class LoginResponseHandler : PhotonServerHandler
    {
        public LoginResponseHandler(PhotonApplication application)
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
            get { return (int)MessageSubCode.Login; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Log.DebugFormat("Login Response Handler.OnHandleMessage");
            if (message.Parameters.ContainsKey((byte)ClientParameterCode.PeerId))
            {
                var peerId = new Guid((byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);
                PhotonClientPeer peer;

                Server.ConnectionCollection<PhotonConnectionCollection>().Clients.TryGetValue(peerId, out peer);

                if (peer != null)
                {
                    if (message.Parameters.ContainsKey((byte)ClientParameterCode.UserId))
                    {
                        Log.DebugFormat("Found User {0}", Convert.ToInt32(message.Parameters[(byte)ClientParameterCode.UserId]));

                        // find the user id and put into the character data
                        peer.ClientData<CharacterData>().UserId = Convert.ToInt32(message.Parameters[(byte)ClientParameterCode.UserId]);
                    }

                    // Remove the peer/character id we don't want to send these back to the client
                    message.Parameters.Remove((byte)ClientParameterCode.PeerId);
                    message.Parameters.Remove((byte)ClientParameterCode.UserId);
                    message.Parameters.Remove((byte)ClientParameterCode.CharacterId);

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
