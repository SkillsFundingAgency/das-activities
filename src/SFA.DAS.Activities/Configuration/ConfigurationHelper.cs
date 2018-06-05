using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Activities.Configuration
{
    public static class ConfigurationHelper
    {
        public static T GetConfiguration<T>(string serviceName, string version)
        {
            var environmentName = EnvironmentHelper.GetEnvironmentName();
            var configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            var configurationService = new ConfigurationService(configurationRepository, new ConfigurationOptions(serviceName, environmentName, version));
            var config = configurationService.Get<T>();

            return config;
        }
    }
}