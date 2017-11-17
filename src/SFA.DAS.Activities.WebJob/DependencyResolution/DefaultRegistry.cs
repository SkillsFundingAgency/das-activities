using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using SFA.DAS.Activities.Application.Configurations;
using SFA.DAS.Activities.Application.Repositories;
using SFA.DAS.Activities.DataAccess.Repositories;
using SFA.DAS.Activities.Infrastructure.DependencyResolution.Configuration;
using SFA.DAS.Activities.WebJob.MessageProcessors;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.AzureServiceBus.Helpers;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Activities.WebJob.DependencyResolution
{


    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {

            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS."));
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
            });
            For<IProvideSettings>().Use(c => new AppConfigSettingsProvider(new MachineSettings("Activities:")));
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

                        return new TopicSubscriberFactory(x.GetInstance<IActivitiesConfiguration>().ServiceBusConnectionString, subscriptionName);

                });
            }

            For<ILog>().Use(x => new NLogLogger(x.ParentType, null, null)).AlwaysUnique();
            For<IActivitiesRepository>().Use<ActivitiesRepository>();
        }

    }
}