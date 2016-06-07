using FluentNHibernate.Mapping;
using HiveCity.Server.Sub.Common.Data.NHibernate;

namespace HiveCity.Server.Sub.Common.Data.Mapping
{
    public class PlayerCharacterMap : ClassMap<PlayerCharacter>
    {
        public PlayerCharacterMap()
        {
            Id(x => x.Id).Column("Id");

            Map(x => x.Name).Column("Name");
            Map(x => x.Class).Column("Class");
            Map(x => x.Level).Column("Level");
            Map(x => x.Chapter).Column("Chapter");
            Map(x => x.Stats).Column("Stats");
            Map(x => x.Position).Column("Position");
            References(x => x.UserId).Column("UserId");

            Table("Characters");
        }
    }
}
