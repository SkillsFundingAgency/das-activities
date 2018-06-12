using SFA.DAS.Activities.DependencyResolver;
using SFA.DAS.Activities.Worker;
using SFA.DAS.IntegrityChecker.Worker.Infrastructure;
using StructureMap;

namespace SFA.DAS.Activities.Host
{
    public static class ActivitiesHost
    {
        public static IContainer InitializeIoC()
        {
            return new Container(c =>
            {
                c.AddRegistry<ElasticRegistry>();
                c.AddRegistry<CosmosRegistry>();
                c.AddRegistry<AzureRegistry>();
                c.AddRegistry<IntegrityCheckRegistry>();
                c.AddRegistry<IntegrityCheckerRegistry>();
                c.AddRegistry<ActivitiesWorkerRegistry>();
            });
        }
    }
}