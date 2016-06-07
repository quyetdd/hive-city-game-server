// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseForwardHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Take response from subservers back to clients
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

    using global::Photon.SocketServer;

    public class ResponseForwardHandler : DefaultResponseHandler
    {
        public ResponseForwardHandler(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Response; }
        }

        public override byte Code
        {
            get { return (byte)(ClientOperationCode.Chat | ClientOperationCode.Login | ClientOperationCode.Region); }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        /// <summary>
        /// The on handle message.
        /// </summary>
        /// <remarks>
        /// Message not passing guids properly in serialization.
        /// </remarks>
        /// <param name="message">
        /// message
        /// </param>
        /// <param name="serverPeer">
        /// Server peer
        /// </param>
        /// <returns>
        /// </returns>
        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            // Looking for a specific peer id else ignore the message
            if (message.Parameters.ContainsKey((byte)ClientParameterCode.PeerId))
            {
                var peerId = new Guid((byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);

                Log.DebugFormat("Looking for peerId {0}", peerId);
                PhotonClientPeer peer;
                Server.ConnectionCollection<PhotonConnectionCollection>().Clients.TryGetValue(peerId, out peer);

                if (peer != null)
                {
                    Log.DebugFormat("Found Peer");

                    // Stop hackers knowing internal peerids
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
                        peer.SendOperationResponse(new OperationResponse(message.Code, message.Parameters), new SendParameters());
                    }
                }
            }

            return true;
        }
    }
}
