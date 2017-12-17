using System.Linq;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.Activities.Elastic;
using StructureMap;
using StructureMap.Building.Interception;

namespace SFA.DAS.Activities
{
    public class ActivitiesRegistry : Registry
    {
        public ActivitiesRegistry()
        {
            Scan(s =>
            {
                s.AssemblyContainingType<ActivitiesRegistry>();
                s.AddAllTypesOf<IIndexMapper>();
            });
            
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