﻿using SFA.DAS.Activities.Configuration;

namespace SFA.DAS.Activities.Worker
{
    public class ActivitiesWorkerConfiguration : IElasticConfiguration, IMessageServiceBusConfiguration
    {
        public string ElasticUrl { get; set; }
        public string ElasticUsername { get; set; }
        public string ElasticPassword { get; set; }
        public string MessageServiceBusConnectionString { get; set; }
    }
}