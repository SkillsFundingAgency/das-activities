using System.Net;
using SFA.DAS.Activities.AcceptanceTests.Azure;
using SFA.DAS.Activities.Client;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.Worker;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Activities.AcceptanceTests
{
    public class ActivitiesAcceptanceTestRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Activities.Worker";
        private const string Version = "1.0";

        public ActivitiesAcceptanceTestRegistry()
        {
            var environmentConfig = new EnvironmentConfiguration { EnvironmentName = "AT" };
            var config = ConfigurationHelper.GetConfiguration<ActivitiesWorkerConfiguration>(ServiceName, Version);

            IncludeRegistry<ActivitiesRegistry>();
            IncludeRegistry<ActivitiesClientRegistry>();
            For<IElasticConfiguration>().Use(config);
            For<IEnvironmentConfiguration>().Use(environmentConfig);
            For<ILog>().Use(c => new NLogLogger(c.ParentType, null, null)).AlwaysUnique();
            For<IMessageServiceBusConfiguration>().Use(config);
            For<IAzureTopicMessageBus>().Use(new AzureTopicMessageBus(config.MessageServiceBusConnectionString, ""));

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
        }
    }
}