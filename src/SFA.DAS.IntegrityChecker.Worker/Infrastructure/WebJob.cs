using SFA.DAS.Activities.DependencyResolver;
using StructureMap;

namespace SFA.DAS.IntegrityChecker.Worker.Infrastructure
{
    public static class WebJob
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
            });
        }
    }
}