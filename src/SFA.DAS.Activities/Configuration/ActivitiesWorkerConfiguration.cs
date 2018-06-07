namespace SFA.DAS.Activities.Configuration
{
    public class ActivitiesWorkerConfiguration : IMessageServiceBusConfiguration, ICosmosConfiguration, IElasticConfiguration, IAzureBlobStorageConfiguration, IIntegrityCheckConfiguration
    {
        public ActivitiesWorkerConfiguration()
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
        public string MessageServiceBusConnectionString { get; set; }
        public string LogStorageConnectionString { get; set; }
        public int CosmosPageSize { get; set; }
        public int ElasticPageSize { get; set; }
        public int? MaxInspections { get; set; }
        public int? MaxDiscrepancies { get; set; }
    }
}