// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopMove.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the StopMove type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.ServerEvents
{
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Proxy.Common.MessageObjects;

    public class StopMove : ServerPacket
    {
        public StopMove(CCharacter character) : base(ClientEventCode.ServerPacket, MessageSubCode.StopMove)
        {
            this.AddParameter(character.ObjectId, ClientParameterCode.ObjectId);
            this.AddSerializedParameter((PositionData)character.Destination, ClientParameterCode.Object);
        }

        public StopMove(int objectId, float x, float y, float z, short heading) :
            base(ClientEventCode.ServerPacket, MessageSubCode.StopMove)
        {
            this.AddParameter(objectId, ClientParameterCode.ObjectId);
            this.AddSerializedParameter(new PositionData(x, y, z, heading), ClientParameterCode.Object);
        }
    }
}
