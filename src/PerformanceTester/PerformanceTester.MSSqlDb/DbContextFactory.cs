using System.Data.SqlClient;
using PerformanceTester.Types;

namespace PerformanceTester.MSSqlDb
{
    public class DbContextFactory : IDbContextFactory
    {
        private readonly IConfigProvider _configProvider;

        public DbContextFactory(IConfigProvider configProvider)
        {
            _configProvider = configProvider;
        }

        public ActivityDbContext Create()
        {
            var config = _configProvider.Get<MsSqlConfig>();
            var connection = new SqlConnection(config.ConnectionString);
            return new ActivityDbContext(connection);
        }
    }
}