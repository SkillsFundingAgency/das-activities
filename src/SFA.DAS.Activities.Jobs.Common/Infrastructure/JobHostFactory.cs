using System.Diagnostics;
using Microsoft.Azure.WebJobs;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.Jobs.Common.Infrastructure
{
    public class JobHostFactory : IJobHostFactory
    {
        private readonly IWebJobConfiguration _configuration;
        private readonly ILog _logger;

        public JobHostFactory(IWebJobConfiguration configuration, ILog logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public JobHost CreateJobHost()
        {
            _logger.Debug($"DashboardConnectionString: {_configuration.DashboardConnectionString}");
            _logger.Debug($"StorageConnectionString: {_configuration.StorageConnectionString}");

            JobHostConfiguration config = new JobHostConfiguration
            {
                DashboardConnectionString = _configuration.DashboardConnectionString,
                StorageConnectionString = _configuration.StorageConnectionString,
            };

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            _logger.Debug($"IsDevelopment: {config.IsDevelopment}");

            config.Tracing.ConsoleLevel = TraceLevel.Verbose;
            config.UseTimers();

            JobHost host = new JobHost(config);

            return host;
        }
    }
}