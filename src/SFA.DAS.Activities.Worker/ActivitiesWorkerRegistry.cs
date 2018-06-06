using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.DependencyResolver;
using SFA.DAS.Activities.Worker.ActivitySavers;
using SFA.DAS.Activities.Worker.ObjectMappers;
using SFA.DAS.Activities.Worker.Policies;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using StructureMap;
using Topshelf;

namespace SFA.DAS.Activities.Worker
{
    public class ActivitiesWorkerRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Activities";
        private const string Version = "1.0";

        private static readonly ILog Log = new NLogLogger(typeof(ActivitiesWorkerRegistry));

        public ActivitiesWorkerRegistry()
        {

            var config = ConfigurationHelper.GetConfiguration<ActivitiesWorkerConfiguration>(ServiceName, Version);

            For<IActivityMapper>().Use<ActivityMapper>();
            For<ServiceControl>().Use<Service>();
            For<IActivitySaver>().Use<ActivitySaver>().Singleton();
            For<IMessageContextProvider>().Use<MessageContextProvider>().Singleton();
            For<IMessageServiceBusConfiguration>().Use(config);
            Policies.Add(new MessageSubscriberPolicy(ServiceName, config, Log));

            Scan(s =>
            {
                s.AssemblyContainingType<ActivitiesWorkerRegistry>();
                s.AddAllTypesOf<IMessageProcessor>();
            });
        }
    }
}