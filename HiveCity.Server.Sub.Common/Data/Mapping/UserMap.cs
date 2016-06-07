using FluentNHibernate.Mapping;
using HiveCity.Server.Sub.Common.Data.NHibernate;

namespace HiveCity.Server.Sub.Common.Data.Mapping
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Id(x => x.Id).Column("Id");
            Map(x => x.Username).Column("Username");
            Map(x => x.Password).Column("Password");
            Map(x => x.Salt).Column("Salt");
            Map(x => x.Email).Column("Email");
            Map(x => x.Algorithm).Column("Algorithm");
            Map(x => x.Created).Column("CreatedAt");
            Map(x => x.Updated).Column("UpdatedAt");
            Table("Users");
        }
    }
}
