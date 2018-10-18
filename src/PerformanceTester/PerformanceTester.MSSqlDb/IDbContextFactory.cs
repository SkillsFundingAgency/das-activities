namespace PerformanceTester.MSSqlDb
{
    public interface IDbContextFactory
    {
        ActivityDbContext Create();
    }
}