using SFA.DAS.Activities.DependencyResolver;
using SFA.DAS.Activities.Jobs.Common.DependencyResolution;
using StructureMap;

namespace SFA.DAS.Activities.MessageHandlers.DependencyResolution
{
    public static class IoC
    {
        public static IContainer InitializeIoC()
        {
            return new Container(c =>
            {
                c.AddRegistry<JobsCommonRegistry>();
                c.AddRegistry<ElasticRegistry>();
                c.AddRegistry<CosmosRegistry>();
                c.AddRegistry<ActivitiesWorkerRegistry>();
            });
        }
    }
}