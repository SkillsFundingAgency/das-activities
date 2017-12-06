using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Azure;
using SFA.DAS.Acitivities.Core.Configuration;
using SFA.DAS.Activities.Application.Configurations;
using SFA.DAS.Activities.Application.Repositories;
using SFA.DAS.Activities.DataAccess.Repositories;
using SFA.DAS.Activities.Infrastructure.Configuration;
using SFA.DAS.Activities.Infrastructure.MessageProcessors;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.AzureServiceBus.Helpers;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.Messaging.Interfaces;
using StructureMap;

namespace SFA.DAS.Activities.WorkerRole.DependencyResolution
{
    public class InfrastructureRegistry : Registry
    {
        public InfrastructureRegistry()
        {
            For<ISettingsProvider>().Use(BuildSettingsProvider()).Singleton();
            For<IMessageProcessor>().Use<PayeSchemeCreatedMessageProcessor>().Named("PayeSchemeCreatedMessageProcessor");

            if (Debugger.IsAttached)
            {
                For<IMessageSubscriberFactory>().Use("", x =>
                {
                    var groupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "EAS_Queues");
                    return new FileSystemMessageSubscriberFactory(groupFolder);
                });
            }
            else
            {
                For<IMessageSubscriberFactory>().Use("", x =>
                {
                    var subscriptionName = TopicSubscriptionHelper.GetMessageGroupName("Activity_PayeSchemeCreatedMessageProcessor");

                    return new TopicSubscriberFactory(x.GetInstance<IOptions<ServiceBusConfiguration>>().Value.ConnectionString, subscriptionName);

                });
            }

            For<IActivitiesRepository>().Use<ActivitiesRepository>();

        }

        private static SettingsProvider BuildSettingsProvider()
        {
            return new SettingsBuilder()
                .AddProvider(new CloudConfigProvider()
                    .AddSection<EnvironmentConfiguration>("Environment"))
                .AddProvider(new AppSettingsProvider())
                .AddProvider(new TableStorageProvider(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"))
                    .AddSection("SFA.DAS.Activities"))
                .Build();
        }
    }
}