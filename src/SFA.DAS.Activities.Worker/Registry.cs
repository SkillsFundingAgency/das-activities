using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using SFA.DAS.Activities.Client;
using SFA.DAS.Activities.Worker.Configuration;
using SFA.DAS.Activities.Worker.MessageProcessors;
using SFA.DAS.Activities.Worker.Services;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.AzureServiceBus.Helpers;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.Worker
{
    public class Registry : StructureMap.Registry
    {
        public Registry()
        {
            var settingsProvider = new SettingsBuilder()
                .AddProvider(new CloudConfigProvider()
                    .AddSection<ActivitiesClientConfiguration>("ElasticSearch")
                    .AddSection<EnvironmentConfiguration>("Environment")
                    .AddSection<ServiceBusConfiguration>("ServiceBus"))
                .AddProvider(new AppSettingsProvider())
                .Build();

            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS."));
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<IActivitiesService>().Use<ActivitiesService>();
            For<ILog>().Use(c => new NLogLogger(c.ParentType, null, null)).AlwaysUnique();
            For<IMessageProcessor>().Use<PayeSchemeCreatedMessageProcessor>().Named("PayeSchemeCreatedMessageProcessor");
            For<ISettingsProvider>().Use(settingsProvider).Singleton();
            For<EnvironmentConfiguration>().Use(c => c.GetInstance<ISettingsProvider>().GetSection<EnvironmentConfiguration>("Environment"));
            For<ActivitiesClientConfiguration>().Use(c => c.GetInstance<ISettingsProvider>().GetSection<ActivitiesClientConfiguration>("ElasticSearch"));
            For<ServiceBusConfiguration>().Use(c => c.GetInstance<ISettingsProvider>().GetSection<ServiceBusConfiguration>("ServiceBus"));

            if (Debugger.IsAttached)
            {
                For<IMessageSubscriberFactory>().Use("", c =>
                {
                    var groupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EAS_Queues");

                    if (!Directory.Exists(groupFolder))
                    {
                        Directory.CreateDirectory(groupFolder);
                    }

                    return new FileSystemMessageSubscriberFactory(groupFolder);
                });
            }
            else
            {
                For<IMessageSubscriberFactory>().Use("", c =>
                {
                    var subscriptionName = TopicSubscriptionHelper.GetMessageGroupName("Activity_PayeSchemeAddedMessageProcessor");
                    var connectionString = c.GetInstance<ServiceBusConfiguration>().ConnectionString;

                    if (string.IsNullOrEmpty(connectionString))
                    {
                        throw new ConfigurationErrorsException("Connection string was empty.");
                    }

                    c.GetInstance<ILog>().Info("Creating subscriber", new Dictionary<string, object>
                    {
                        { "ConnectionString", connectionString },
                        { "SubscriptionName", subscriptionName }
                    });

                    return new TopicSubscriberFactory(connectionString, subscriptionName);

                });
            }
        }
    }
}