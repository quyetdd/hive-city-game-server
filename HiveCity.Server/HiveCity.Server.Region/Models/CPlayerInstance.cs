// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CPlayerInstance.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the CPlayerInstance type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models
{
    using System;
    using System.Linq;

    using ExitGames.Logging;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Application;
    using HiveCity.Server.Photon.Client;
    using HiveCity.Server.Photon.Server;
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Proxy.Common.MessageObjects;
    using HiveCity.Server.Region.Models.Interfaces;
    using HiveCity.Server.Region.Models.KnownList;
    using HiveCity.Server.Region.Models.ServerEvents;
    using HiveCity.Server.Region.Models.Stats;
    using HiveCity.Server.Sub.Common;
    using HiveCity.Server.Sub.Common.Data.NHibernate;

    using global::Photon.SocketServer;

    public class CPlayerInstance : CPlayable, IPlayer, IClientData
    {
        public CPlayerInstance(Region region, PlayerKnownList playerKnownList, IStatHolder stats, IPhysics physics)
            : base(region, playerKnownList, stats)
        {
            Physics = physics;
            // Todo: Temp
            Physics.MoveSpeed = Stats.GetStat<MoveSpeed>();
            Destination = new Position();
        }

        public IPhysics Physics { get; set; }

        protected ILogger Log = LogManager.GetCurrentClassLogger();

        public SubServerClientPeer Client { get; set; }
        public PhotonServerPeer ServerPeer { get; set; }
        public int? UserId { get; set; }
        public int? CharacterId { get; set; }
      
        // Hijack the physics to set our position
        // physics position
        public override Position Position
        {
            get
            {
                base.Position = Physics.Position;
                return base.Position;
            }
            set
            {
                base.Position = value;
                if (Physics != null)
                {
                    Physics.Position = value;
                }

            }
        }

        public override MoveDirection Direction
        {
            get
            {
                base.Direction = Physics.Direction;
                return base.Direction;

            }
            set { base.Direction = value; }
        }

        public override bool Moving
        {
            get
            {
                base.Moving = Physics.Moving;
                return base.Moving;
            }
            set { base.Moving = value; }
        }

        public new PlayerKnownList KnownList
        {
            get { return ObjectKnownList as PlayerKnownList; }
            set { ObjectKnownList = value; }
        }

        public override void BroadcastMessage(ServerPacket packet)
        {
            if (packet.SendToSelf)
            {
                SendPacket(packet);
            }

            foreach (CPlayerInstance player in KnownList.KnownPlayers.Values)
            {
                player.SendPacket(packet);
            }
        }

        public override void SendPacket(ServerPacket packet)
        {
            if (Client != null)
            {
                // TODO: This may not work properly - Watch this!
                //var proxyClient = Client.Server.ConnectionCollection<SubServerConnectionCollection>().Clients.FirstOrDefault(x => x.Value == Client);
                //packet.AddParameter(proxyClient.Key.ToString(), ClientParameterCode.PeerId);
                //ServerPeer.SendEvent(new EventData(packet.Code, packet.Parameters), new SendParameters());
  
                // TODO: This may not work properly - Watch this!
                var proxyClient = Client.Server.ConnectionCollection<SubServerConnectionCollection>().Clients.FirstOrDefault(x => x.Value == Client);
                Log.DebugFormat("Sending {0} to {0}", packet.GetType().Name, Name);
                packet.AddParameter(proxyClient.Key.ToByteArray(), ClientParameterCode.PeerId);
                ServerPeer.SendEvent(new EventData(packet.Code, packet.Parameters), new SendParameters());
            }
        }

        public override void SendInfo(IObject obj)
        {
            obj.SendPacket(new CharacterInfoUpdate(this));
        }

        public void BroadcastUserInfo()
        {
            SendPacket(new UserInfoUpdate(this)); // Full player stats
            BroadcastMessage(new CharacterInfoUpdate(this)); // Other people see this about the player
        }

        /// <summary>
        /// Stop things running after log out
        /// </summary>
        public override void DeleteMe()
        {
            CleanUp();
            Store();
            base.DeleteMe();
        }

        public void CleanUp()
        {
            Log.DebugFormat("Logging off");

            // about auto attack or casting
            StopMove(null);

            // remove temporary items - session based
            // remove from LFG/Battlegrounds

            // Stop all timers
            // Stop Crafting

            Target = null;

            // Stop temporay effects

            // Decay from server
            Decay();

            // Remove from all groups
            // Unsommon pets
            // Notify guild of logoff
            // Cancel trading
            // Notify friend list of logoff
        }

        public void Store()
        {
            // Save character to database

            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var user = session.QueryOver<User>().Where(w => w.Id == UserId).SingleOrDefault();
                        var character =
                            session.QueryOver<PlayerCharacter>()
                                .Where(w => w.UserId == user && w.Name == Name)
                                .SingleOrDefault();

                        character.Level = (int) Stats.GetStat<Level>();
                        string position = Position.Serialize();
                        character.Position = position;
                        // Store Stats
                        character.Stats = Stats.SerializeStats();

                        session.Save(character);
                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void Restore(int objectId)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var character =
                        session.QueryOver<PlayerCharacter>()
                            .Where(w => w.Id == objectId)
                            .List()
                            .FirstOrDefault();
                    transaction.Commit();
                    if (character != null)
                    {
                        ObjectId = objectId;
                        Name = character.Name;

                        // Appearance
                        // Level
                        Stats.SetStat<Level>(character.Level);

                        // XP

                        // Position
                        if (!string.IsNullOrEmpty(character.Position))
                        {
                            Position = Position.Deserialize(character.Position);
                        }
                        else
                        {
                            Position = new Position(0,50,0); // Default spawn position
                        }
            
                        // Build

                        // Title
                        if (!string.IsNullOrEmpty(character.Stats))
                        {
                            Stats.DeserializeStats(character.Stats);
                        }
                        
                        // Equipment 
                        // Inventory
                        // Effects

                        // Social - guild notify, groups notify

                        Log.DebugFormat("Max Hp: {0}", Stats.GetStat<Level5Hp>());
                    }
                    else
                    {
                        Log.FatalFormat("CPlayerInstance - Should never reach character not found in the database.");
                    }
                }
            }
        }
    }
}
