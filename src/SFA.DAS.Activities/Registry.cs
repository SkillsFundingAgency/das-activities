using System;
using System.Diagnostics;
using System.IO;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.Data;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.AzureServiceBus.Helpers;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities
{
    public class Registry : StructureMap.Registry
    {
        public Registry()
        {
            var settingsProvider = new SettingsBuilder()
                .AddProvider(new CloudConfigProvider()
                    .AddSection<ElasticConfiguration>("ElasticSearch")
                    .AddSection<EnvironmentConfiguration>("Environment")
                    .AddSection<ServiceBusConfiguration>("ServiceBus"))
                .AddProvider(new AppSettingsProvider())
                .Build();

            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS."));
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<IActivitiesRepository>().Use<ActivitiesRepository>();
            For<ILog>().Use(c => new NLogLogger(c.ParentType, null, null)).AlwaysUnique();
            For<ISettingsProvider>().Use(settingsProvider).Singleton();
            For<EnvironmentConfiguration>().Use(c => c.GetInstance<ISettingsProvider>().GetSection<EnvironmentConfiguration>("Environment"));
            For<ElasticConfiguration>().Use(c => c.GetInstance<ISettingsProvider>().GetSection<ElasticConfiguration>("ElasticSearch"));
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
                    var subscriptionName = TopicSubscriptionHelper.GetMessageGroupName("Activity_PayeSchemeCreatedMessageProcessor");

                    return new TopicSubscriberFactory(c.GetInstance<ServiceBusConfiguration>().ConnectionString, subscriptionName);

                });
            }
        }
    }
}