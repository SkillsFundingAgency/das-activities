using System.Collections.Generic;

namespace SFA.DAS.Activities.Configuration
{
    public interface ISettingsProvider
    {
        IDictionary<string, object> Settings { get; }
    }
}