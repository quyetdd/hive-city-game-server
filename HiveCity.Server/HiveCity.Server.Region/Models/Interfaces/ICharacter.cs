// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICharacter.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the ICharacter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.Interfaces
{
    using System.Collections.Generic;

    using HiveCity.Server.Proxy.Common.MessageObjects;
    using HiveCity.Server.Region.Models.ServerEvents;

    // Specific for characters
    public interface ICharacter
    {
        IObject Target { get; set; }
        
        // ObjectId
        int TargetId { get; }

        bool IsTeleporting { get; }

        bool IsDead { get; }

        Position Destination { get; set; }

        MoveDirection Direction { get; set; }

        bool Moving { get; set; }
        
        // Targeted person - Damage, Hp Group status for individuals
        IEnumerable<ICharacter> StatusListeners { get; }

        IStatHolder Stats { get; }

        // Damage updates, health updates etc.
        void BroadcastMessage(ServerPacket packet);

        void SendMessage(string text);

        // Summoning, going into instances, etc.
        void Teleport(Position pos);

        void Teleport(float x, float y, float z, short heading);

        void Teleport(float x, float y, float z);
       
        // Resurrection
        void Teleport(ITeleportType teleportType);

        // Pvp / Mobs / Drop loot / Handle non dying npcs
        bool Die(ICharacter killer); 

        void StopMove(Position pos);

        // XP etc
        void CalculateRewards(ICharacter killer); 

        // Player Characters
        void BroadcastStatusUpdate(); 

        // type = hp, etc
        // Generic base update to status listeners
        void UpdateAndBroadcastStatusUpdate(int broadcastType); 
        
        void SendStateToPlayer(IObject owner);
    }
}
