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
            IncludeRegistry<ActivitiesRegistry>();

            Scan(scan =>
            {
                scan.AssembliesAndExecutablesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS."));
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
                scan.AddAllTypesOf<IMessageProcessor>();
            });

            For<IActivityMapper>().Use<ActivityMapper>();
            For<ILog>().Use(c => new NLogLogger(c.ParentType, null, null)).AlwaysUnique();
        }
    }
}