// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerMovementHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the PlayerMovementHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Handlers
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
    using HiveCity.Server.Region.Models;
    using HiveCity.Server.Region.Operations;

    using global::Photon.SocketServer;

    using IMessage = HiveCity.Server.Framework.IMessage;

    public class PlayerMovementHandler : PhotonServerHandler
    {
        public PlayerMovementHandler(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return (byte)ClientOperationCode.Region; }
        }

        public override int? SubCode
        {
            get { return (int)MessageSubCode.PlayerMovement; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var para = new Dictionary<byte, object>()
            {
                {(byte) ClientParameterCode.PeerId, message.Parameters[(byte) ClientParameterCode.PeerId]},
                {(byte) ClientParameterCode.SubOperationCode,message.Parameters[(byte) ClientParameterCode.SubOperationCode]},
            };


            var operation = new PlayerMovementOperation(serverPeer.Protocol, message);
            if (!operation.IsValid)
            {
                Log.ErrorFormat(operation.GetErrorMessage());
                serverPeer.SendOperationResponse(
                    new OperationResponse(message.Code)
                        {
                            ReturnCode = (int)ErrorCode.OperationInvalid,
                            DebugMessage = operation.GetErrorMessage(),
                            Parameters = para
                        },
                    new SendParameters());

                return true;
            }

            var peerId = new Guid((byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);
            var clients = Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
            var instance = clients[peerId].ClientData<CPlayerInstance>();

            var serializer = new XmlSerializer(typeof(PlayerMovement));
            var reader = new StringReader(operation.PlayerMovement);
            var playerMovement = (PlayerMovement)serializer.Deserialize(reader);

            instance.Physics.Movement = playerMovement;
            instance.Facing = playerMovement.Facing;
            
            return true;
        }
    }
}
