using FluentNHibernate.Mapping;
using HiveCity.Server.Sub.Common.Data.NHibernate;

namespace HiveCity.Server.Sub.Common.Data.Mapping
{
    public class UserProfileMap : ClassMap<UserProfile>
    {
        public UserProfileMap()
        {
            Id(x => x.Id).Column("Id");
            Map(x => x.CharacterSlots).Column("CharacterSlots");
            References(x => x.UserId).Column("UserId");
            Table("UserProfiles");
        }
    }
}
