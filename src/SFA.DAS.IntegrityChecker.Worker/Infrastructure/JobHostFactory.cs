using System.Diagnostics;
using Microsoft.Azure.WebJobs;

namespace SFA.DAS.Activities.Jobs.Infrastructure
{
    public class JobHostFactory : IJobHostFactory
    {
        private readonly IWebJobConfiguration _configuration;

        public JobHostFactory(IWebJobConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JobHost CreateJobHost()
        {
            JobHostConfiguration config = new JobHostConfiguration
            {
                DashboardConnectionString = _configuration.DashboardConnectionString,
                StorageConnectionString = _configuration.StorageConnectionString,
            };

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            config.Tracing.ConsoleLevel = TraceLevel.Verbose;
            config.UseTimers();

            JobHost host = new JobHost(config);

            return host;
        }
    }
}