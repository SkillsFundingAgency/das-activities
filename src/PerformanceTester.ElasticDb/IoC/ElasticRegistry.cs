using PerformanceTester.ElasticDb.IndexMappers;
using PerformanceTester.ElasticDb.Interfaces;
using StructureMap;

namespace PerformanceTester.ElasticDb.IoC
{
    public class ElasticRegistry : Registry
    {
        public ElasticRegistry()
        {
            For<IElasticClientFactory>().Use<ElasticClientFactory>().Singleton();
            For<IIndexMapper>().Use<ActivitiesIndexMapper>().Singleton();
        }
    }
}
