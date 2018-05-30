using System.Data;
using SFA.DAS.Activities.DependencyResolver;
using SFA.DAS.IntegrityChecker.Worker.Infrastructure;
using StructureMap;

namespace SFA.DAS.IntegrityChecker.Worker
{
    public static class WebJob
    {
        public static IContainer InitializeIoC()
        {
            return new Container(c =>
            {
                c.AddRegistry<ElasticRegistry>();
                c.AddRegistry<CosmosRegistry>();
                c.AddRegistry<IntegrityCheckerRegistry>();
            });
        }
    }
}