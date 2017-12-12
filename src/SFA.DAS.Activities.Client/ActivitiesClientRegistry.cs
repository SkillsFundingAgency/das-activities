using Nest;
using SFA.DAS.Activities.Client.Elastic;

namespace SFA.DAS.Activities.Client
{
    public class ActivitiesClientRegistry : StructureMap.Registry
    {
        public ActivitiesClientRegistry()
        {
            For<IElasticClientFactory>().Use<ElasticClientFactory>().Singleton();
            For<IElasticClient>().Use(c => c.GetInstance<IElasticClientFactory>().GetClient()).Singleton();
        }
    }
}