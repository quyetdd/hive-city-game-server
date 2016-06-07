// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TeleportToLocation.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the TeleportToLocation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.ServerEvents
{
    using HiveCity.Server.Proxy.Common;

    public class TeleportToLocation : ServerPacket
    {
        public TeleportToLocation(CObject obj, float x, float y, float z, short heading) 
            : this(obj, new Position(x,y,z,heading))
        {
            
        }

        public TeleportToLocation(CObject obj, Position pos)
            : base(ClientEventCode.ServerPacket, MessageSubCode.TeleportToLocation)
        {
            this.AddParameter(obj.ObjectId, ClientParameterCode.ObjectId);
            this.AddSerializedParameter(pos, ClientParameterCode.Object);
        }
    }
}
