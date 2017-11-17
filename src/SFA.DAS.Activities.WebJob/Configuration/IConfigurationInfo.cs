using System;

namespace SFA.DAS.Activities.WebJob.Configuration
{
    public interface IConfigurationInfo<out T>
    {
        T GetConfiguration(string serviceName);
        T GetConfiguration(string serviceName, Action<string> action);
    }
}
