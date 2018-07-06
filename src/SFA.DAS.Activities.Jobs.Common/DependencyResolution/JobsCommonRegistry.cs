using Microsoft.Azure;
using SFA.DAS.Activities.Jobs.Common.Infrastructure;
using StructureMap;

namespace SFA.DAS.Activities.Jobs.Common.DependencyResolution
{
    public class JobsCommonRegistry : Registry
    {
        public JobsCommonRegistry()
        {
            For<IWebJobConfiguration>().Use(new WebJobConfig
            {
                DashboardConnectionString = CloudConfigurationManager.GetSetting("DashboardConnectionString"),
                StorageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString")
            });

            For<IJobHostFactory>().Use<JobHostFactory>().Singleton();
        }
    }
}
