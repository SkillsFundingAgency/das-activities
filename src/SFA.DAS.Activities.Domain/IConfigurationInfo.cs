using System;

namespace SFA.DAS.Activities.Domain
{
    public interface IConfigurationInfo<out T>
    {
        T GetConfiguration(string serviceName);
        T GetConfiguration(string serviceName, Action<string> action);
    }
}
