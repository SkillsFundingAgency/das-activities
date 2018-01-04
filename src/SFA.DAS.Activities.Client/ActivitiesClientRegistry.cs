using SFA.DAS.Activities.Configuration;
using StructureMap;

namespace SFA.DAS.Activities.Client
{
    public class ActivitiesClientRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Activities.Client";
        private const string Version = "1.0";

        public ActivitiesClientRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<ActivitiesClientConfiguration>(ServiceName, Version);

            IncludeRegistry<ActivitiesRegistry>();
            For<IActivitiesClient>().Use<ActivitiesClient>();
            For<IElasticConfiguration>().Use(config);
        }
    }
}