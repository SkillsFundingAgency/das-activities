namespace PerformanceTester.Types
{
    public interface IConfigProvider
    {
        TConfigType Get<TConfigType>() where TConfigType : class, new();
    }
}