using HiveCity.Server.Region.Models.Interfaces;
using HiveCity.Server.Region.Models.KnownList;

namespace HiveCity.Server.Region.Models
{
    public class CPlayable : CCharacter, IPlayable
    {
        public CPlayable(Region region, PlayableKnownList playableKnownList, IStatHolder stats)
            : base(region, playableKnownList, stats)
        {
        }

        public new PlayableKnownList KnownList
        {
            get { return ObjectKnownList as PlayableKnownList; }
            set { ObjectKnownList = value; }
        }
    }
}
