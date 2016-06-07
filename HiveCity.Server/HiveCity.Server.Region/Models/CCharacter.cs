// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CCharacter.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the CCharacter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models
{
    using System.Collections.Generic;

    using HiveCity.Server.Proxy.Common.MessageObjects;
    using HiveCity.Server.Region.Models.Interfaces;
    using HiveCity.Server.Region.Models.KnownList;
    using HiveCity.Server.Region.Models.ServerEvents;

    public class CCharacter : CObject, ICharacter
    {
        private IObject target;

        protected CCharacter(Region region, CharacterKnownList objectKnownList, IStatHolder stats)
            : base(region, objectKnownList)
        {
            this.Stats = stats;
            this.Stats.Character = this;
            this.StatusListeners = new List<ICharacter>();
        }

        private new CharacterKnownList KnownList
        {
            get { return ObjectKnownList as CharacterKnownList; }
        }

        public IObject Target
        {
            get
            {
                return this.target;
            }

            set
            {
                if (value != null && !value.IsVisible)
                {
                    value = null;
                }

                if (value != null && value != this.target)
                {
                    // player knows who is attacking them
                    this.KnownList.AddKnownObject(this.target);

                    // attacker can see targeter
                    value.KnownList.AddKnownObject(value);
                }
            }
        }

        public int TargetId
        {
            get
            {
                if (this.Target != null)
                {
                    return this.Target.ObjectId;
                }

                return -1; // No target
            }
        }

        public bool IsTeleporting { get; private set; }

        public bool IsDead { get; private set; }

        public Position Destination { get; set; }

        public virtual MoveDirection Direction { get; set; }

        public int Facing { get; set; }

        public virtual bool Moving { get; set; }

        public IEnumerable<ICharacter> StatusListeners { get; private set; }

        public IStatHolder Stats { get; protected set; }

        // Tell npc spawner to kick off timer to respawn
        public delegate void DeathListener(ICharacter killer);

        public DeathListener DeathListeners;

        /// <summary>
        /// NPCs use this function for sending messages to players
        /// </summary>
        /// <param name="packet"></param>
        public virtual void BroadcastMessage(ServerPacket packet)
        {
            foreach (CPlayerInstance knownPlayer in this.KnownList.KnownPlayers.Values)
            {
                knownPlayer.SendPacket(packet);
            }
        }

        public virtual void SendMessage(string text)
        {
        }

        public void Teleport(Position pos)
        {
            this.Teleport(pos.X, pos.Y, pos.Z, pos.Heading);
        }

        public void Teleport(float x, float y, float z, short heading)
        {
            this.StopMove(null); // Stop character - move back to non predicted position

            this.IsTeleporting = true;
            this.Target = null;

            this.BroadcastMessage(new TeleportToLocation(this, x, y, z, heading));

            this.Decay();
            Position.XYZ(x, y, z);
            Position.Heading = heading;
        }

        public void Teleport(float x, float y, float z)
        {
            this.Teleport(x, y, z, Position.Heading);
        }

        public void Teleport(ITeleportType teleportType)
        {
            this.Teleport(teleportType.GetNearestTeleportLocation(this));
        }

        /// <summary>
        /// Death process
        /// </summary>
        /// <param name="killer">Who killed you</param>
        /// <returns></returns>
        public virtual bool Die(ICharacter killer)
        {
            if (this.IsDead)
            {
                return false;
            }

            //// Stats.OnDeath();

            this.IsDead = true;
            if (this.DeathListeners != null)
            {
                this.DeathListeners(killer);
            }

            this.Target = null;
            this.StopMove(null);

            // Stop buffs if end upon death
            //// Effects.StopAllEffectsThroughDeath();

            this.CalculateRewards(killer);
            this.BroadcastStatusUpdate();
            ////Region.OnDeath(this);

            return true;
        }

        public void StopMove(Position pos)
        {
            if (pos != null)
            {
                this.Destination = pos;
                this.Position = pos;
            }
            else
            {
                this.Destination = this.Position;
            }

            this.BroadcastMessage(new StopMove(this));
        }

        public virtual void CalculateRewards(ICharacter killer)
        {
        }

        public void BroadcastStatusUpdate()
        {
            foreach (var statusListener in this.StatusListeners)
            {
                statusListener.BroadcastMessage(new StatusUpdate(this));
            }
        }

        public virtual void UpdateAndBroadcastStatusUpdate(int broadcastType)
        {
        }

        public virtual void SendStateToPlayer(IObject owner)
        {
            // owner.SendPacket(new MoveToLocation(this));
        }

        /// <summary>
        /// Deregister the player
        /// </summary>
        public virtual void DeleteMe()
        {
        }
    }
}
