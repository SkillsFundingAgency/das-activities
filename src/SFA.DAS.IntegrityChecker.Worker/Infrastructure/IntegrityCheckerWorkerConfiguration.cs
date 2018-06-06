using SFA.DAS.Activities.Configuration;

namespace SFA.DAS.IntegrityChecker.Worker.Infrastructure
{
    public class IntegrityCheckerWorkerConfiguration : ICosmosConfiguration, IElasticConfiguration, IAzureBlobStorageConfiguration
    {
        public string ElasticUrl { get; set; }
        public string ElasticUsername { get; set; }
        public string ElasticPassword { get; set; }
        public string CosmosDatabase { get; set; }
        public string CosmosEndpointUrl { get; set; }
        public string CosmosPrimaryKey { get; set; }
        public string CosmosCollectionName { get; set; }
        public string LogStorageConnectionString { get; set; }
    }
}