using Nest;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.IndexMappers;
using SFA.DAS.Activities.Worker;
using SFA.DAS.Activities.Worker.ActivitySavers;
using SFA.DAS.Elastic;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Activities.DependencyResolver
{
    public class CommonWorkerRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Activities";
        private const string Version = "1.0";

        private static readonly ILog Log = new NLogLogger(typeof(CommonWorkerRegistry));

        public CommonWorkerRegistry()
        {

            var config = ConfigurationHelper.GetConfiguration<ActivitiesWorkerConfiguration>(ServiceName, Version);
            
            var elasticConfig = new ElasticConfiguration()
                .UseSingleNodeConnectionPool(config.ElasticUrl)
                .ScanForIndexMappers(typeof(ActivitiesIndexMapper).Assembly)
                .OnRequestCompleted(r => Log.Debug(r.DebugInformation));

            if (!string.IsNullOrWhiteSpace(config.ElasticUsername) && !string.IsNullOrWhiteSpace(config.ElasticPassword))
            {
                elasticConfig.UseBasicAuthentication(config.ElasticUsername, config.ElasticPassword);
            }

            For<IElasticClient>().Use(c => c.GetInstance<IElasticClientFactory>().CreateClient()).Singleton();
            For<IElasticClientFactory>().Use(() => elasticConfig.CreateClientFactory()).Singleton();
            For<ILog>().Use(c => new NLogLogger(c.ParentType, null, null)).AlwaysUnique();
            For<ICosmosClient>().Use<CosmosClient>().Singleton();
        }
    }
}