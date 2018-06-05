using System;
using System.Linq;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.IntegrityChecker;
using SFA.DAS.Activities.IntegrityChecker.Fixers;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;
using SFA.DAS.Activities.IntegrityChecker.Repositories;
using SFA.DAS.Activities.IntegrityChecker.Utils;
using StructureMap.TypeRules;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace SFA.DAS.IntegrityChecker.Worker.Infrastructure
{
    public class IntegrityCheckerRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Activities";
        private const string Version = "1.0";

        public IntegrityCheckerRegistry()
        {
            For<IWebJobConfiguration>().Use(new WebJobConfig
            {
                DashboardConnectionString = CloudConfigurationManager.GetSetting("DashboardConnectionString"),
                StorageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString")
            });

            For<IJobHostFactory>().Use<JobHostFactory>().Singleton();
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

    internal class SingletonConvention<TPluginFamily> : IRegistrationConvention
    {
        public void ScanTypes(TypeSet types, Registry registry)
        {
            foreach (var type in types.AllTypes())
            {
                if (type.CanBeCreated())
                {
                    registry.For(typeof(TPluginFamily)).Singleton().Use(type);
                }
            }
        }
    }
}