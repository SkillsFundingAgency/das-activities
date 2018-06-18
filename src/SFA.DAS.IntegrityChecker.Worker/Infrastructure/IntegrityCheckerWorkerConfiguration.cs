using SFA.DAS.Activities.Configuration;

namespace SFA.DAS.Activities.Jobs.Infrastructure
{
    public class IntegrityCheckerWorkerConfiguration : ICosmosConfiguration, IElasticConfiguration, IAzureBlobStorageConfiguration, IIntegrityCheckConfiguration
    {
        public IntegrityCheckerWorkerConfiguration()
        {
            CosmosPageSize = 500;
            ElasticPageSize = 2500;
        }

        public string ElasticUrl { get; set; }
        public string ElasticUsername { get; set; }
        public string ElasticPassword { get; set; }
        public string CosmosDatabase { get; set; }
        public string CosmosEndpointUrl { get; set; }
        public string CosmosPrimaryKey { get; set; }
        public string CosmosCollectionName { get; set; }
        public string LogStorageConnectionString { get; set; }
        public int CosmosPageSize { get; set; }
        public int ElasticPageSize { get; set; }
        public int? MaxInspections { get; set; }
        public int? MaxDiscrepancies { get; set; }
    }
}