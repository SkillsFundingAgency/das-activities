using System.Collections.Generic;
using System.Configuration;

namespace SFA.DAS.Activities.Worker.Configuration
{
    public class AppSettingsProvider : IProvideSettings
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