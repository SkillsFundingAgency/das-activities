using System.Collections.Generic;
using System.Configuration;

namespace SFA.DAS.Activities.Configuration
{
    public class AppSettingsProvider : ISettingsProvider
    {
        public IDictionary<string, object> Settings { get; set; } = GetSettings();

        private static IDictionary<string, object> GetSettings()
        {
            var settings = new Dictionary<string, object>();

            foreach (var key in ConfigurationManager.AppSettings.AllKeys)
            {
                settings.Add(key, ConfigurationManager.AppSettings[key]);
            }

            return settings;
        }
    }
}