using System.Collections.Generic;

namespace SFA.DAS.Activities.Worker.Configuration
{
    public interface IProvideSettings
    {
        IDictionary<string, object> Settings { get; }
    }
}