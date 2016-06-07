using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace HiveCity.Server.Sub.Common
{
    public class NHibernateHelper
    {
        public NHibernateHelper()
        {
            InitializeSessionFactory();
        }

        private static ISessionFactory _sessionFactory;

        private static ISessionFactory SessionFactory {
            get
            {
                if (_sessionFactory == null)
                    InitializeSessionFactory();

                return _sessionFactory;
            }
        }

        private static void InitializeSessionFactory()
        {
            _sessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                .ConnectionString(@"data source=.;initial catalog=HiveCityDb;Integrated Security=True"))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<NHibernateHelper>())
                .BuildSessionFactory();
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
