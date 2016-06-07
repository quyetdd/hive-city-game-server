// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerKnownList.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the PlayerKnownList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.KnownList
{
    using HiveCity.Server.Region.Models.Interfaces;
    using HiveCity.Server.Region.Models.ServerEvents;

    public class PlayerKnownList : PlayableKnownList
    {
        public override bool AddKnownObject(IObject obj)
        {
            if (!base.AddKnownObject(obj))
            {
                return false;
            }

            obj.SendInfo(this.Owner);

            var character = obj as ICharacter;
            if (character != null)
            {
                character.SendStateToPlayer(this.Owner);
            }

            return true;
        }

        public override bool RemoveKnownObject(IObject obj)
        {
            if (!base.RemoveKnownObject(obj))
            {
                return false;
            }

            obj.SendPacket(new DeleteObject(obj));
            return true;
        }

        public override int DistanceToForgetObject(IObject obj)
        {
            // Scaling of things around the player
            // 400 metres try to remember
            if (KnownObjects.Count <= 25)
            {
                return 400;
            }

            if (KnownObjects.Count <= 35)
            {
                return 350;
            }
            
            if (KnownObjects.Count <= 70)
            {
                return 295;
            }

            return 235;
        }

        public override int DistanceToWatchObject(IObject obj)
        {
            // Scaling of things around the player
            // 400 metres try to remember
            // 1 region is 1km2
            if (KnownObjects.Count <= 25)
            {
                return 340;
            }

            if (KnownObjects.Count <= 35)
            {
                return 290;
            }

            if (KnownObjects.Count <= 70)
            {
                return 230;
            }

            return 170;
        }
    }
}
