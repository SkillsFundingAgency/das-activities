using Nest;
using NLog.Fluent;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.IndexMappers;
using SFA.DAS.Elastic;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Activities.Client
{
    public class ActivitiesClientRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Activities.Client";
        private const string Version = "1.0";

        private static readonly ILog Log = new NLogLogger(typeof(ActivitiesClientRegistry));

        public ActivitiesClientRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<ActivitiesClientConfiguration>(ServiceName, Version);

            var elasticConfig = new ElasticConfiguration()
                .UseSingleNodeConnectionPool(config.ElasticUrl)
                .ScanForIndexMappers(typeof(ActivitiesIndexMapper).Assembly)
                .OnRequestCompleted(r => Log.Debug(r.DebugInformation));

            if (!string.IsNullOrWhiteSpace(config.ElasticUsername) && !string.IsNullOrWhiteSpace(config.ElasticPassword))
            {
                elasticConfig.UseBasicAuthentication(config.ElasticUsername, config.ElasticPassword);
            }

            var elasticClientFactory = elasticConfig.CreateClientFactory();

            For<IActivitiesClient>().Use<ActivitiesClient>();
            For<IElasticClient>().Use(c => c.GetInstance<IElasticClientFactory>().CreateClient()).Singleton();
            For<IElasticClientFactory>().Use(elasticClientFactory);
        }
    }
}