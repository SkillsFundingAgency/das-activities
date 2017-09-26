using System;

namespace SFA.DAS.Activities.Worker.Configuration
{
    public interface IConfigurationInfo<out T>
    {
        T GetConfiguration(string serviceName);
        T GetConfiguration(string serviceName, Action<string> action);
    }
}
