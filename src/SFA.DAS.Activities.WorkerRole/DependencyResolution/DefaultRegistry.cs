using System.Diagnostics;
using System.IO;
using SFA.DAS.Acitivities.Core.Configuration;
using StructureMap;
using SFA.DAS.Activities.Application.Configurations;
using SFA.DAS.Activities.Application.Repositories;
using SFA.DAS.Activities.DataAccess.Repositories;
using SFA.DAS.Activities.Infrastructure.MessageProcessors;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.AzureServiceBus.Helpers;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.WorkerRole.DependencyResolution
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

            For<ILog>().Use(x => new NLogLogger(x.ParentType, null, null)).AlwaysUnique();
        }
    }
}