// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListCharacters.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the ListCharacters type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Login.Operations
{
    using HiveCity.Server.Framework;
    using HiveCity.Server.Proxy.Common;

    using global::Photon.SocketServer;

    using global::Photon.SocketServer.Rpc;

    public class ListCharacters : Operation
    {
        public ListCharacters(IRpcProtocol protocol, IMessage message) : base(protocol, new OperationRequest(message.Code, message.Parameters))
        {
        }

        [DataMember(Code = (byte)ClientParameterCode.UserId, IsOptional = false)]
        public int UserId { get; set; }
    }
}
