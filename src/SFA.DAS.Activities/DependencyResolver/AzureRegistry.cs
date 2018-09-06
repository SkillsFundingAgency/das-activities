using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.IntegrityChecker;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;
using SFA.DAS.Activities.IntegrityChecker.Repositories;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Activities.DependencyResolver
{
    public class AzureRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Activities";
        private const string Version = "1.0";

        private static readonly ILog Log = new NLogLogger(typeof(AzureRegistry));

        public AzureRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<ActivitiesWorkerConfiguration>(ServiceName, Version);

            For<IAzureBlobStorageConfiguration>().Use(config);
            For<IAzureBlobRepository>().Use<AzureBlobRepository>().Singleton();
        }
    }
}