using SFA.DAS.Activities.Configuration;
using StructureMap;

namespace SFA.DAS.Activities.Client
{
    public class ActivitiesClientRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Activities.Client";

        public ActivitiesClientRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<ActivitiesClientConfiguration>(ServiceName);

            IncludeRegistry<ActivitiesRegistry>();
            For<IActivitiesClient>().Use<ActivitiesClient>();
            For<IElasticConfiguration>().Use(config);
        }
    }
}