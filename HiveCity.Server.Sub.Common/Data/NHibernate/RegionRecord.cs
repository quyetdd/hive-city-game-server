using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HiveCity.Server.Sub.Common.Data.NHibernate
{
    public class RegionRecord
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string ColliderPath { get; set; }
        public virtual string Boundary { get; set; }

    }
}
