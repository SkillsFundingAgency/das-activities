using System.Collections.Generic;

namespace SFA.DAS.Activities.Configuration
{
    public interface IProvideSettings
    {
        IDictionary<string, object> Settings { get; }
    }
}