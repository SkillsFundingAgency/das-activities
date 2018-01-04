using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.Worker.ObjectMappers;
using SFA.DAS.Activities.Worker.Policies;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Activities.Worker
{
    public class ActivitiesWorkerRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Activities.Worker";
        private const string Version = "1.0";

        public ActivitiesWorkerRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<ActivitiesWorkerConfiguration>(ServiceName, Version);

            IncludeRegistry<ActivitiesRegistry>();
            For<IActivityMapper>().Use<ActivityMapper>();
            For<IElasticConfiguration>().Use(config);
            For<IMessageServiceBusConfiguration>().Use(config);
            For<ILog>().Use(c => new NLogLogger(c.ParentType, null, null)).AlwaysUnique();
            Policies.Add(new MessageSubscriberPolicy<ActivitiesWorkerConfiguration>(ServiceName, Version, new NLogLogger(typeof(TopicSubscriberFactory))));

            Scan(s =>
            {
                s.AssemblyContainingType<ActivitiesWorkerRegistry>();
                s.AddAllTypesOf<IMessageProcessor>();
            });
        }
    }
}