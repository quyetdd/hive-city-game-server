// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharacterInfoUpdate.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the CharacterInfoUpdate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.ServerEvents
{
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Proxy.Common.MessageObjects;

    public class CharacterInfoUpdate : ServerPacket
    {
        public CharacterInfoUpdate(CPlayerInstance player) 
            : base(ClientEventCode.ServerPacket, MessageSubCode.CharacterInfo, false)
        {
            this.AddParameter(player.ObjectId, ClientParameterCode.ObjectId);
            this.AddCharacterInfo(player);
        }

        public void AddCharacterInfo(CPlayerInstance player)
        {
            // Other players see this about the player
            // On instantiation
            var info = new CharInfo
            {
                Position = player.Position,
                Name = player.Name,

                // Race, chapter, class, title, company
                // Inventory - visible slots armour/weapon

                // effects pvp flag, debuffs, buffs

                // movement speed for smoothing/calculation

                // action/emote walk, run sit
            };

            this.AddSerializedParameter(info, ClientParameterCode.Object, false);
        }
    }
}
