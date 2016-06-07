using HiveCity.Server.Framework;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace HiveCity.Server.Sub.Common.Operations
{
    public class RegisterSubServer : Operation
    {
        public RegisterSubServer(IRpcProtocol rpcProtocol, IMessage message)
            : base(rpcProtocol, new OperationRequest(message.Code, message.Parameters))
        {

        }

        public RegisterSubServer(IRpcProtocol rpcProtocol, OperationRequest operationResponse)
            : base(rpcProtocol, operationResponse)
        {
        }

        public RegisterSubServer()
        {
        }

        [DataMember(Code = (byte)ServerParameterCode.RegisterSubServerOperation, IsOptional = false)]
        public string RegisterSubServerOperation { get; set; }

    }
}
