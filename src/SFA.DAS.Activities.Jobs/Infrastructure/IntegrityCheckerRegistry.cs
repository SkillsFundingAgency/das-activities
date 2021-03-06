using SFA.DAS.Activities.IntegrityChecker;
using SFA.DAS.Activities.IntegrityChecker.FixLogging;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;
using SFA.DAS.Activities.IntegrityChecker.Utils;
using SFA.DAS.Activities.Jobs.Common.Infrastructure;
using StructureMap;
using StructureMap.TypeRules;

namespace SFA.DAS.Activities.Jobs.Infrastructure
{
    public class IntegrityCheckerRegistry : Registry
    {
        public IntegrityCheckerRegistry()
        {
            For<IActivitiesScan>().Use<ActivitiesScan>().Singleton();
            For<IActivitiesFix>().Use<ActivitiesFix>().Singleton();
            For<IActivityDiscrepancyQueue>().Use<ActivityDiscrepancyQueue>().Singleton();
            For<IActivityDiscrepancyFinder>().Use<ActivityDiscrepancyFinder>().Singleton();
            For<IFixActionLogger>().Use<FixActionLogger>().Singleton();

            Scan(scan =>
            {
                scan.AssemblyContainingType<FixActionLogger>();
                scan.Include(type => type.IsConcreteAndAssignableTo(typeof(IActivityDiscrepancyFixer)));
                scan.With(new SingletonConvention<IActivityDiscrepancyFixer>());
            });
        }
    }
}