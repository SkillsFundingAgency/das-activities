using System;
using SFA.DAS.Configuration;

namespace NuGetProject
{
    public interface IConfigurationRepositoryInfo<out T> where T : IConfiguration
    {
        T GetConfigurationRepository(string serviceName);
        T GetConfigurationRepository(string serviceName, Action<string> action);
    }
}
