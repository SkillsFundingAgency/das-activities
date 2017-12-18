using SFA.DAS.Activities.Worker.MessageProcessors;
using SFA.DAS.Activities.Worker.ObjectMappers;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Activities.Worker
{
    public class ActivitiesWorkerRegistry : Registry
    {
        public ActivitiesWorkerRegistry()
        {
            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS."));
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
                scan.AddAllTypesOf<IMessageProcessor>();
            });

            For<IActivityMapper>().Use<ActivityMapper>();
            For<ILog>().Use(c => new NLogLogger(c.ParentType, null, null)).AlwaysUnique();
            For<IMessageProcessor>().Use<PayeSchemeCreatedMessageProcessor>().Named("PayeSchemeCreatedMessageProcessor");
        }
    }
}