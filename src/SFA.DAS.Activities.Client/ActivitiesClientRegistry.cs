using System.Linq;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.Activities.Client.Elastic;
using StructureMap;
using StructureMap.Building.Interception;

namespace SFA.DAS.Activities.Client
{
    public class ActivitiesClientRegistry : Registry
    {
        public ActivitiesClientRegistry()
        {
            Scan(s =>
            {
                s.AssemblyContainingType<ActivitiesClientRegistry>();
                s.AddAllTypesOf<IIndexMapper>();
            });

            For<IActivitiesClient>().Use<ActivitiesClient>();
            For<IElasticClientFactory>().Use<ElasticClientFactory>().Singleton();

            For<IElasticClient>()
                .Use(c => c.GetInstance<IElasticClientFactory>().GetClient())
                .Singleton()
                .InterceptWith(new ActivatorInterceptor<IElasticClient>((context, client) => 
                    Task.WaitAll(context.GetAllInstances<IIndexMapper>().Select(m => m.EnureIndexExists(client)).ToArray()))
                );
        }
    }
}