using System;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Activities.Configuration
{
    public static class ConfigurationHelper
    {
        public static T GetConfiguration<T>(string serviceName, string version)
        {
            var environmentName = GetEnvironmentName();
            var configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            var configurationService = new ConfigurationService(configurationRepository, new ConfigurationOptions(serviceName, environmentName, version));
            var config = configurationService.Get<T>();

            return config;
        }

        public static string GetEnvironmentName()
        {
            var environmentName = Environment.GetEnvironmentVariable("DASENV");

            if (string.IsNullOrEmpty(environmentName))
            {
                environmentName = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            return environmentName;
        }
    }
}