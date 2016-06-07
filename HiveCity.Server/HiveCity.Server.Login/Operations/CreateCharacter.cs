// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateCharacter.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the CreateCharacter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Login.Operations
{
    using HiveCity.Server.Framework;
    using HiveCity.Server.Proxy.Common;

    using global::Photon.SocketServer;

    using global::Photon.SocketServer.Rpc;

    public class CreateCharacter : Operation
    {
        public CreateCharacter(IRpcProtocol protocol, IMessage message)
            : base(protocol, new OperationRequest(message.Code, message.Parameters))
        {
        }

        [DataMember(Code = (byte)ClientParameterCode.UserId, IsOptional = false)]
        public int UserId { get; set; }
        
        [DataMember(Code = (byte)ClientParameterCode.CharacterCreateDetails, IsOptional = false)]
        public string CharacterCreateDetails { get; set; }
    }
}
