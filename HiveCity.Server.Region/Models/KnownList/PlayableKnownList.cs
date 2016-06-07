// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayableKnownList.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the PlayableKnownList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.KnownList
{
    using System.Threading.Tasks;

    using HiveCity.Server.Region.Models.Interfaces;

    public class PlayableKnownList : CharacterKnownList
    {
        public override void FindObjects()
        {
            Parallel.ForEach(
                Owner.Region.VisibleObjects,
                obj =>
                    {
                        if (obj != Owner)
                        {
                            AddKnownObject(obj);

                            // So other characters know about the player
                            if (obj is ICharacter)
                            {
                                obj.KnownList.AddKnownObject(Owner);
                            }
                        }
                    });
        }
    }
}
