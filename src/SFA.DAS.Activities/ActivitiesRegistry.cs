using Nest;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.Elastic;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Activities
{
    public class ActivitiesRegistry : Registry
    {
        private static readonly object Lock = new object();

        private static IElasticClientFactory _elasticClientFactory;

        public ActivitiesRegistry()
        {
            var environmentConfig = new EnvironmentConfiguration { EnvironmentName = ConfigurationHelper.GetEnvironmentName() };

            For<IElasticClientFactory>().Use(c => GetElasticClientFactory(c)).Singleton();
            For<IElasticClient>().Use(c => c.GetInstance<IElasticClientFactory>().GetClient());
            For<IEnvironmentConfiguration>().Use(environmentConfig);

            Scan(s =>
            {
                s.AssemblyContainingType<ActivitiesRegistry>();
                s.AddAllTypesOf<IIndexMapper>();
            });
        }

        private IElasticClientFactory GetElasticClientFactory(IContext context)
        {
            lock (Lock)
            {
                if (_elasticClientFactory == null)
                {
                    var elasticConfig = context.GetInstance<IElasticConfiguration>();
                    var environmentConfig = context.GetInstance<IEnvironmentConfiguration>();
                    var indexMappers = context.GetAllInstances<IIndexMapper>();
                    var log = context.GetInstance<ILog>();

                    _elasticClientFactory = new ElasticClientFactory(elasticConfig, environmentConfig, indexMappers, log);
                }
            }

            return _elasticClientFactory;
        }
    }
}