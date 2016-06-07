// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginSecurely.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the LoginSecurely type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Login.Operations
{
    using HiveCity.Server.Framework;
    using HiveCity.Server.Proxy.Common;

    using global::Photon.SocketServer;

    using global::Photon.SocketServer.Rpc;

    public class LoginSecurely : Operation
    {
        public LoginSecurely(IRpcProtocol rpcProtocol, IMessage message)
            : base(rpcProtocol, new OperationRequest(message.Code, message.Parameters))
        {
        }
        
        [DataMember(Code = (byte)ClientParameterCode.UserName, IsOptional = false)]
        public string UserName { get; set; }

        [DataMember(Code = (byte)ClientParameterCode.Password, IsOptional = false)]
        public string Password { get; set; }
    }
}
