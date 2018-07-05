using SFA.DAS.Activities.DependencyResolver;
using StructureMap;

namespace SFA.DAS.Activities.MessageHandlers
{
    public static class ActivityWorker
    {
        public static IContainer InitializeIoC()
        {
            return new Container(c =>
            {
                c.AddRegistry<ElasticRegistry>();
                c.AddRegistry<CosmosRegistry>();
                c.AddRegistry<ActivitiesWorkerRegistry>();
            });
        }
    }
}