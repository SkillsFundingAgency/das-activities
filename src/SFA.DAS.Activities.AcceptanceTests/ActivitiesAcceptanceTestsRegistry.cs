﻿using System.Net;
using Nest;
using SFA.DAS.Activities.AcceptanceTests.Azure;
using SFA.DAS.Activities.Client;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.IndexMappers;
using SFA.DAS.ApiSubstitute.Utilities;
using SFA.DAS.Elastic;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Activities.AcceptanceTests
{
    public class ActivitiesAcceptanceTestsRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Activities";
        private const string Version = "1.0";

        private static readonly ILog Log = new NLogLogger(typeof(ActivitiesAcceptanceTestsRegistry));

        public ActivitiesAcceptanceTestsRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<ActivitiesAcceptanceTestsConfiguration>(ServiceName, Version);

            var elasticConfig = new ElasticConfiguration()
                .OverrideEnvironmentName("AT")
                .UseSingleNodeConnectionPool(config.ElasticUrl)
                .ScanForIndexMappers(typeof(ActivitiesIndexMapper).Assembly)
                .OnRequestCompleted(r => Log.Debug(r.DebugInformation));

            if (!string.IsNullOrWhiteSpace(config.ElasticUsername) && !string.IsNullOrWhiteSpace(config.ElasticPassword))
            {
                elasticConfig.UseBasicAuthentication(config.ElasticUsername, config.ElasticPassword);
            }

            For<IActivitiesClient>().Use<ActivitiesClient>();
            For<IAzureTopicMessageBus>().Use(new AzureTopicMessageBus(config.MessageServiceBusConnectionString, ""));
            For<IElasticClient>().Use(c => c.GetInstance<IElasticClientFactory>().CreateClient()).Singleton();
            For<IElasticClientFactory>().Use(() => elasticConfig.CreateClientFactory()).Singleton();
            For<ILog>().Use(c => new NLogLogger(c.ParentType, null, null)).AlwaysUnique();
            For<IMessageServiceBusConfiguration>().Use(config);
            For<IObjectCreator>().Use<ObjectCreator>();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
        }
    }
}