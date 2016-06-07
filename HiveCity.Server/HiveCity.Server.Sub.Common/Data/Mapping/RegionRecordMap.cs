using FluentNHibernate.Mapping;
using HiveCity.Server.Sub.Common.Data.NHibernate;

namespace HiveCity.Server.Sub.Common.Data.Mapping
{
    public class RegionRecordMap : ClassMap<RegionRecord>
    {
        public RegionRecordMap()
        {
            Id(x => x.Id).Column("Id");
            Map(x => x.Name).Column("Name");
            Map(x => x.ColliderPath).Column("ColliderPath");
            Map(x => x.Boundary).Column("Boundary");
            Table("Regions");
        }
    }
}
