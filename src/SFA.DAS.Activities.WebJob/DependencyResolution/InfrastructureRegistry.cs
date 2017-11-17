using SFA.DAS.Activities.Application.Configurations;
using SFA.DAS.Activities.Infrastructure.DependencyResolution.Configuration;
using SFA.DAS.Activities.WebJob.Configuration;
using StructureMap;

namespace SFA.DAS.Activities.WebJob.DependencyResolution
{
    public class InfrastructureRegistry : Registry
    {
        public InfrastructureRegistry()
        {
            For<IProvideSettings>().Use(c => new AppConfigSettingsProvider(new MachineSettings("DAS_")));
        }
    }
}