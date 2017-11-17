using SFA.DAS.Activities.Application.Configurations;
using SFA.DAS.Activities.Infrastructure.DependencyResolution.Configuration;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Activities.WebJob.DependencyResolution
{


    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {

            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS."));
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
            });
            For<IProvideSettings>().Use(c => new AppConfigSettingsProvider(new MachineSettings("Activities:")));
            RegisterLogger();
        }

        private void RegisterLogger()
        {
            For<ILog>().Use(x => new NLogLogger(
                x.ParentType,
                null, null)).AlwaysUnique();
        }
    }
}