using HiveCity.Server.Framework;
using HiveCity.Server.Proxy.Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace HiveCity.Server.World.Operations
{
    public class RegionInfoOperation : Operation
    {
        public RegionInfoOperation(IRpcProtocol protocol, IMessage message)
            : base(protocol, new OperationRequest(message.Code, message.Parameters))
        {
        }

        [DataMember(Code = (byte)ClientParameterCode.Object, IsOptional = false)]
        public string RegionInfo { get; set; }

    }
}
