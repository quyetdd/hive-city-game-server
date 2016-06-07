using HiveCity.Server.Proxy.Common;

namespace HiveCity.Server.Region.Models.ServerEvents
{
    public class StatusUpdate : ServerPacket
    {
        public StatusUpdate(CCharacter character)
            : base(ClientEventCode.ServerPacket, MessageSubCode.StatusUpdate)
        {
            AddParameter(character.ObjectId, ClientParameterCode.ObjectId);
        }
    }
}
