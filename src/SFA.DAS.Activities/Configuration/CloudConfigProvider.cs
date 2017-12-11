using System.Collections.Generic;
using Microsoft.Azure;

namespace SFA.DAS.Activities.Configuration
{
    public class CloudConfigProvider : IProvideSettings
    {
        public IDictionary<string, object> Settings { get; set; } = new Dictionary<string, object>();

        public CloudConfigProvider AddSection<T>(string name)
        {
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                var key = $"{name}:{propertyInfo.Name}";
                var setting = CloudConfigurationManager.GetSetting(key.Replace(":","_"));

                if (setting != null)
                {
                    Settings.Add(key, setting);
                }
            }

            return this;
        }
    }
}