using SFA.DAS.Acitivities.Core.Configuration;
using SFA.DAS.Activities.Application.Configurations;
using SFA.DAS.Activities.Infrastructure.Configuration;
using StructureMap;

namespace SFA.DAS.Activities.WorkerRole.DependencyResolution
{
    public class ApplicationRegistry : Registry
    {
        public ApplicationRegistry()
        {
            For<IOptions<EnvironmentConfiguration>>().Use(c =>
                c.GetInstance<ISettingsProvider>().GetSection<EnvironmentConfiguration>("Environment"));
            For<IOptions<ElasticConfiguration>>().Use(c =>
                c.GetInstance<ISettingsProvider>().GetSection<ElasticConfiguration>("ElasticSearch"));
            For<IOptions<ServiceBusConfiguration>>().Use(c =>
                c.GetInstance<ISettingsProvider>().GetSection<ServiceBusConfiguration>("ServiceBus"));
        }
    }
}