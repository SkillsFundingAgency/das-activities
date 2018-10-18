using Nest;

namespace PerformanceTester.ElasticDb.Interfaces
{
    public interface IElasticClientFactory
    {
        IElasticClient CreateClient();
    }
}