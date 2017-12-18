using System.Linq;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.Elastic;
using StructureMap;
using StructureMap.Building.Interception;

namespace SFA.DAS.Activities
{
    public class ActivitiesRegistry : Registry
    {
        public ActivitiesRegistry()
        {
            var settings = new SettingsBuilder()
                .AddProvider(new CloudConfigurationProvider()
                    .AddSection<ActivitiesElasticConfiguration>("ElasticSearch")
                    .AddSection<ActivitiesEnvironmentConfiguration>("Environment")
                    .AddSection<ActivitiesServiceBusConfiguration>("ServiceBus"))
                .AddProvider(new AppSettingsProvider())
                .Build();

            Scan(s =>
            {
                s.AssemblyContainingType<ActivitiesRegistry>();
                s.AddAllTypesOf<IIndexMapper>();
            });
            
            For<IElasticClientFactory>().Use<ElasticClientFactory>().Singleton().Ctor<IElasticConfiguration>().Is(c => c.GetInstance<ActivitiesElasticConfiguration>());

            For<IElasticClient>()
                .Use(c => c.GetInstance<IElasticClientFactory>().GetClient())
                .Singleton()
                .InterceptWith(new ActivatorInterceptor<IElasticClient>((context, client) => 
                    Task.WaitAll(context.GetAllInstances<IIndexMapper>().Select(m => m.EnureIndexExists(client)).ToArray()))
                );

            For<ISettings>().Use(settings);
            For<ActivitiesElasticConfiguration>().Use(c => c.GetInstance<ISettings>().GetSection<ActivitiesElasticConfiguration>("ElasticSearch"));
            For<ActivitiesEnvironmentConfiguration>().Use(c => c.GetInstance<ISettings>().GetSection<ActivitiesEnvironmentConfiguration>("Environment"));
            For<ActivitiesServiceBusConfiguration>().Use(c => c.GetInstance<ISettings>().GetSection<ActivitiesServiceBusConfiguration>("ServiceBus"));
        }
    }
}