using HiveCity.Server.Framework;
using HiveCity.Server.Photon.Application;
using HiveCity.Server.Photon.Server;
using HiveCity.Server.Proxy.Common;
using HiveCity.Server.Proxy.Common.MessageObjects;
using HiveCity.Server.Region.Models;
using HiveCity.Server.World.Models;
using HiveCity.Server.World.Operations;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace HiveCity.Server.World.Handlers
{
    public class ServerAreaHandler : PhotonServerHandler
    {
        public ServerAreaHandler(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte)ClientEventCode.ServerPacket; }
        }

        public override int? SubCode
        {
            get { return (int)MessageSubCode.RegionInfo; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Log.DebugFormat("Recieved Region Infomation");
            var operation = new RegionInfoOperation(serverPeer.Protocol, message);
            if (operation.IsValid)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<PositionData>));
                StringReader reader = new StringReader(operation.RegionInfo);
                List<Position> posData = new List<Position>();
                foreach (var positionData in serializer.Deserialize(reader) as List<PositionData>)
                {
                    posData.Add(new Position(positionData.X, positionData.Y, positionData.Z));
                }
                serverPeer.ServerData<RegionInfo>().Boundary = posData;
            }

            return true;
        }
    }
}
