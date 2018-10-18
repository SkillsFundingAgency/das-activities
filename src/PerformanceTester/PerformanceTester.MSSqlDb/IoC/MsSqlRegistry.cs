using StructureMap;

namespace PerformanceTester.MSSqlDb.IoC
{
    public class MsSqlRegistry : Registry
    {
        public MsSqlRegistry()
        {
            For<IDbContextFactory>().Use<DbContextFactory>().Singleton();
        }
    }
}
