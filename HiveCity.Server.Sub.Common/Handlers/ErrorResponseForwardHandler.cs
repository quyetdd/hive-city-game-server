using HiveCity.Server.Proxy.Common;
using HiveCity.Server.Framework;
using HiveCity.Server.Photon.Application;
using HiveCity.Server.Photon.Server;

namespace HiveCity.Server.Sub.Common.Handlers
{
    public class ErrorResponseForwardHandler: DefaultResponseHandler
    {
        public ErrorResponseForwardHandler(PhotonApplication application) : base(application) { }

        public override MessageType Type
        {
            get { return MessageType.Response; }
        }

        public override byte Code
        {
            get { return (byte) (ClientOperationCode.Chat | ClientOperationCode.Region | ClientOperationCode.Login); }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Log.ErrorFormat("No existing Response Handler: {0}-{1}", message.Code, message.SubCode);
            return true;
        }
    }
}
