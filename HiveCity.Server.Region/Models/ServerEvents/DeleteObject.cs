// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteObject.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the DeleteObject type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.ServerEvents
{
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Region.Models.Interfaces;

    public class DeleteObject : ServerPacket
    {
        public DeleteObject(IObject obj) : base(ClientEventCode.ServerPacket, MessageSubCode.DeleteObject)
        {
            this.AddParameter(obj.ObjectId, ClientParameterCode.ObjectId);
        }
    }
}
