using Nest;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.Elastic;
using StructureMap;

namespace SFA.DAS.Activities
{
    public class ActivitiesRegistry : Registry
    {
        private static readonly object Lock = new object();

        private static IElasticClientFactory _elasticClientFactory;

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
            
            For<IElasticClientFactory>().Use(c => GetElasticClientFactory(c)).Singleton();
            For<IElasticClient>().Use(c => c.GetInstance<IElasticClientFactory>().GetClient());
            For<ISettings>().Use(settings);
            For<ActivitiesElasticConfiguration>().Use(c => c.GetInstance<ISettings>().GetSection<ActivitiesElasticConfiguration>("ElasticSearch"));
            For<ActivitiesEnvironmentConfiguration>().Use(c => c.GetInstance<ISettings>().GetSection<ActivitiesEnvironmentConfiguration>("Environment"));
            For<ActivitiesServiceBusConfiguration>().Use(c => c.GetInstance<ISettings>().GetSection<ActivitiesServiceBusConfiguration>("ServiceBus"));
        }

        private IElasticClientFactory GetElasticClientFactory(IContext context)
        {
            lock (Lock)
            {
                if (_elasticClientFactory == null)
                {
                    var configuration = context.GetInstance<ActivitiesElasticConfiguration>();
                    var indexMappers = context.GetAllInstances<IIndexMapper>();

                    _elasticClientFactory = new ElasticClientFactory(configuration, indexMappers);
                }
            }

            return _elasticClientFactory;
        }
    }
}