using System.Data.Common;
using System.Data.Entity;
using PerformanceTester.Types;

namespace PerformanceTester.MSSqlDb
{
    public class ActivityDbContext : DbContext
    {
        public ActivityDbContext(DbConnection connection) : base(connection, true)
        {
            
        }
        public DbSet<Activity> Activities { get; set; }
    }
}