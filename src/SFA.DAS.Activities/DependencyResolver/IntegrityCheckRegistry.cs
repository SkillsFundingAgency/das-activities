using SFA.DAS.Activities.Configuration;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Activities.DependencyResolver
{
    public class IntegrityCheckRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Activities";
        private const string Version = "1.0";

        private static readonly ILog Log = new NLogLogger(typeof(AzureRegistry));

        public IntegrityCheckRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<ActivitiesWorkerConfiguration>(ServiceName, Version);

            For<IIntegrityCheckConfiguration>().Use(config);
        }
    }
}