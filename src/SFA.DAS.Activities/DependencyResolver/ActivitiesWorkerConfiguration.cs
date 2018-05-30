﻿using SFA.DAS.Activities.Configuration;

namespace SFA.DAS.Activities.Worker
{
    public class ActivitiesWorkerConfiguration : IMessageServiceBusConfiguration, ICosmosConfiguration, IElasticConfiguration
    {
        public string ElasticUrl { get; set; }
        public string ElasticUsername { get; set; }
        public string ElasticPassword { get; set; }
        public string CosmosDatabase { get; set; }
        public string CosmosEndpointUrl { get; set; }
        public string CosmosPrimaryKey { get; set; }
        public string CosmosCollectionName { get; set; }
        public string MessageServiceBusConnectionString { get; set; }
    }
}