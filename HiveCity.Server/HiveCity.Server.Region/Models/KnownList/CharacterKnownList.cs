// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharacterKnownList.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the CharacterKnownList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.KnownList
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using HiveCity.Server.Region.Models.Interfaces;

    public class CharacterKnownList : ObjectKnownList
    {
        public CharacterKnownList()
            : base()
        {
            KnownPlayers = new ConcurrentDictionary<int, IPlayer>();
        }

        public ConcurrentDictionary<int, IPlayer> KnownPlayers { get; set; }

        public override bool AddKnownObject(IObject obj)
        {
            if (!base.AddKnownObject(obj))
            {
                return false;
            }

            var player = obj as IPlayer;

            if (player != null)
            {
                this.KnownPlayers.TryAdd(obj.ObjectId, player);
            }

            return true;
        }

        public override void FindObjects()
        {
            Parallel.ForEach(
                Owner.Region.VisibleObjects,
                obj =>
                {
                    if (obj != Owner)
                    {
                        AddKnownObject(obj);
                    }
                });
        }

        public override void RemoveAllKnownObjects()
        {
            base.RemoveAllKnownObjects();
            
            this.KnownPlayers.Clear();
        }

        public override bool RemoveKnownObject(IObject obj)
        {
            if (!base.RemoveKnownObject(obj))
            {
                return false;
            }

            IPlayer player = obj as IPlayer;
            if (player != null)
            {
                this.KnownPlayers.TryRemove(obj.ObjectId, out player);
            }

            return true;
        }

        public override void ForgetObjects(bool fullCheck)
        {
            if (!fullCheck)
            {
                var knownPlayers = this.KnownPlayers.Values;

                foreach (var knownPlayer in knownPlayers)
                {
                    var player = (IObject)knownPlayer;
                    if (!player.IsVisible || !Util.IsInShortRange(this.DistanceToForgetObject(player), Owner, player, true))
                    {
                        this.RemoveKnownObject(player);
                    }
                }

                return;
            }

            var objects = KnownObjects.Values;
            foreach (var obj in objects)
            {
                // not visible or not in visible range
                if (!obj.IsVisible || !Util.IsInShortRange(this.DistanceToForgetObject(obj), this.Owner, obj, true))
                {
                    this.RemoveKnownObject(obj);

                    if (!(obj is IPlayer))
                    {
                        continue;
                    }

                    IPlayer player;
                    this.KnownPlayers.TryRemove(obj.ObjectId, out player);
                }
            }
        }

        /// <summary>
        /// Gets seperate list from the knownobjects
        /// </summary>
        public List<ICharacter> KnownCharacters
        {
            get { return KnownObjects.Values.Where(obj => obj is ICharacter).Cast<ICharacter>().ToList(); }
        }

        public List<ICharacter> KnownCharactersInRadius(int radius)
        {
            return KnownObjects.Values.Where(obj => obj is ICharacter && Util.IsInShortRange(radius, Owner, obj, true)).Cast<ICharacter>().ToList();
        }

        public List<IPlayer> KnownPlayersInRadius(int radius)
        {
            return this.KnownPlayers.Values.Where(obj => obj is IObject && Util.IsInShortRange(radius, Owner, (IObject)obj, true)).ToList();
        }
    }
}
