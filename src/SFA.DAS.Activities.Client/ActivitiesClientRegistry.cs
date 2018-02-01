using Nest;
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
            For<ActivitiesClientConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<ActivitiesClientConfiguration>(ServiceName, Version)).Singleton();
            For<ElasticConfiguration>().Use(c => GetElasticConfiguration(c.GetInstance<ActivitiesClientConfiguration>())).Singleton();
            For<IActivitiesClient>().Use<ActivitiesClient>();
            For<IElasticClient>().Use(c => c.GetInstance<IElasticClientFactory>().CreateClient()).Singleton();
            For<IElasticClientFactory>().Use(c => c.GetInstance<ElasticConfiguration>().CreateClientFactory()).Singleton();
        }

        private ElasticConfiguration GetElasticConfiguration(ActivitiesClientConfiguration activitiesdClientConfig)
        {
            var elasticConfig = new ElasticConfiguration()
                .UseSingleNodeConnectionPool(activitiesdClientConfig.ElasticUrl)
                .ScanForIndexMappers(typeof(ActivitiesIndexMapper).Assembly)
                .OnRequestCompleted(r => Log.Debug(r.DebugInformation));

            if (!string.IsNullOrWhiteSpace(activitiesdClientConfig.ElasticUsername) && !string.IsNullOrWhiteSpace(activitiesdClientConfig.ElasticPassword))
            {
                elasticConfig.UseBasicAuthentication(activitiesdClientConfig.ElasticUsername, activitiesdClientConfig.ElasticPassword);
            }

            return elasticConfig;
        }
    }
}