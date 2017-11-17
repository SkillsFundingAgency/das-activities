using System.Collections.Generic;

namespace SFA.DAS.Activities.Application.Configurations
{
    public interface IProvideSettings
    {
        string GetSetting(string settingKey);
        string GetNullableSetting(string settingKey);
        IEnumerable<string> GetArray(string settingKey);
    }
}
