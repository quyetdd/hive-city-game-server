// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserInfoUpdate.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserInfoUpdate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.ServerEvents
{
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Proxy.Common.MessageObjects;

    public class UserInfoUpdate : ServerPacket
    {
        public UserInfoUpdate(CPlayerInstance player): base(ClientEventCode.ServerPacket, MessageSubCode.UserInfo)
        {
            this.AddParameter(player.ObjectId, ClientParameterCode.ObjectId);
            this.AddUserInfo(player);
        }

        private void AddUserInfo(CPlayerInstance player)
        {
            // Upon instantiation
            var info = new UserInfo
            {
                Position = player.Position,
                Name = player.Name,

                // Attr - Level, exp, stats
                // Inventory - all equiped items
                // Talents
                // Effects
                // Movement speeds
                // Actions/Emotes
            };

            this.AddSerializedParameter(info, ClientParameterCode.Object, false);
        }
    }
}
