using System;
using NuGetProject.Environment;
using SFA.DAS.Configuration;

namespace NuGetProject
{
    public class ConfigurationRepositoryInfo<T> : IConfigurationRepositoryInfo<T> where T: IConfiguration
    {
        private readonly IEnvironmentInfo _environmentInfo;
        private readonly IConfigurationRepository _configRepository;

        public ConfigurationRepositoryInfo(IEnvironmentInfo environmentInfo, IConfigurationRepository configRepository)
        {
            _environmentInfo = environmentInfo;
            _configRepository = configRepository;
        }

        public IConfigurationRepository GetConfigurationRepository(string serviceName)
        {
            return GetConfigurationRepository(serviceName, null);
        }

        public IConfigurationRepository GetConfigurationRepository(string serviceName, Action<string> action)
        {
            var environment = _environmentInfo.GetEnvironment().EnvironmentName;
            action?.Invoke(environment);

            var configurationService = new ConfigurationService(_configRepository,
                new ConfigurationOptions(serviceName, environment, "1.0"));

            var result = configurationService.Get<T>();

            return result;
        }
    }
}
