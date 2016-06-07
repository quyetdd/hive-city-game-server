// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerMovementOperation.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the PlayerMovementOperation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Operations
{
    using HiveCity.Server.Framework;
    using HiveCity.Server.Proxy.Common;

    using global::Photon.SocketServer;

    using global::Photon.SocketServer.Rpc;

    public class PlayerMovementOperation : Operation
    {
        public PlayerMovementOperation(IRpcProtocol protocol, IMessage message) : base(protocol, new OperationRequest(message.Code, message.Parameters))
        {
        }

        [DataMember(Code = (byte)ClientParameterCode.UserId, IsOptional = false)]
        public int UserId { get; set; }

        [DataMember(Code = (byte)ClientParameterCode.Object, IsOptional = false)]
        public string PlayerMovement { get; set; }
    }
}
