using System.Collections.Generic;
using System.Configuration;
using SFA.DAS.Activities.Application.Configurations;

namespace SFA.DAS.Activities.Infrastructure.Configuration
{
    public class AppSettingsProvider : IProvideSettings
    {
        public IDictionary<string, object> Settings { get; set; } = GetSettings();

        private static IDictionary<string, object> GetSettings()
        {
            var dict = new Dictionary<string, object>();
            foreach (var key in ConfigurationManager.AppSettings.AllKeys)
            {
                dict.Add(key, ConfigurationManager.AppSettings[key]);
            }

            return dict;
        }
    }
}