// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventForwardHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the EventForwardHandler type.
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

    public class EventForwardHandler : DefaultEventHandler
    {
        public EventForwardHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte)(ClientOperationCode.Chat | ClientOperationCode.Login | ClientOperationCode.Region); }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            // Looking for a specific peer id else ignore the message
            if (message.Parameters.ContainsKey((byte) ClientParameterCode.PeerId))
            {
                PhotonClientPeer peer;

                this.Server.ConnectionCollection<PhotonConnectionCollection>().Clients.TryGetValue(new Guid((byte[])message.Parameters[(byte) ClientParameterCode.PeerId]), out peer);

                if (peer != null)
                {
                    message.Parameters.Remove((byte) ClientParameterCode.PeerId);
                    peer.SendEvent(new EventData(message.Code, message.Parameters), new SendParameters());
                }
            }

            return true;
        }
    }
}