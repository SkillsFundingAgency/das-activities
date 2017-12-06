using System.Collections.Generic;

namespace SFA.DAS.Acitivities.Core.Configuration
{
    public interface IProvideSettings
    {
        IDictionary<string, object> Settings { get; }
    }
}