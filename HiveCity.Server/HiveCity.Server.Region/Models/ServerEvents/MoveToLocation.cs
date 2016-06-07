// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoveToLocation.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the MoveToLocation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.ServerEvents
{
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Proxy.Common.MessageObjects;
    using HiveCity.Server.Region.Models.Stats;

    public class MoveToLocation : ServerPacket
    {
        public MoveToLocation(CCharacter character)
            : base(ClientEventCode.ServerPacket, MessageSubCode.MoveToLocation)
        {
            this.AddParameter(character.ObjectId, ClientParameterCode.ObjectId);
            this.AddSerializedParameter(
                new MoveTo
                {
                    CurrentPosition = (PositionData)character.Position,
                    Destination = (PositionData)character.Destination,
                    Facing = character.Facing,
                    Moving = character.Moving,
                    Direction = character.Direction,
                    Speed = character.Stats.GetStat<MoveSpeed>()
                }, 
                ClientParameterCode.Object);
        }
    }
}
