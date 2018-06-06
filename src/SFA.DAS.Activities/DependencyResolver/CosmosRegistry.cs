using SFA.DAS.Activities.ActivitySavers;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;
using SFA.DAS.Activities.IntegrityChecker.Repositories;
using SFA.DAS.Activities.IntegrityChecker.Utils;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Activities.DependencyResolver
{
    public class CosmosRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Activities";
        private const string Version = "1.0";

        private static readonly ILog Log = new NLogLogger(typeof(CosmosRegistry));

        public CosmosRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<ActivitiesWorkerConfiguration>(ServiceName, Version);

            For<ICosmosConfiguration>().Use(config);
            For<ICosmosClient>().Use<CosmosClient>().Singleton();

            For<ILog>().Use(c => new NLogLogger(c.ParentType, null, null)).AlwaysUnique();
            For<ICosmosActivityDocumentRepository>().Use<CosmosActivityDocumentRepository>();

            For<IDocumentCollectionConfigurator>().Use<DocumentCollectionConfigurator>();
        }
    }
}