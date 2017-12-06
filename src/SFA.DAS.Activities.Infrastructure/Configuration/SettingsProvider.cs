using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Acitivities.Core.Configuration;

namespace SFA.DAS.Activities.Infrastructure.Configuration
{
    public class SettingsProvider : ISettingsProvider
    {
        private Dictionary<string, object> values;

        public SettingsProvider(Dictionary<string, object> values)
        {
            this.values = values;
        }

        public IOptions<T> GetSection<T>(string name) where T : new()
        {
            var section = new T();

            foreach (var propertyInfo in typeof(T).GetProperties().Where(x => x.CanWrite))
            {
                var setter = propertyInfo.GetSetMethod();
                var key = $"{name}:{propertyInfo.Name}";
                if (values.ContainsKey(key) && setter != null)
                {
                    var value = values[key];
                    setter.Invoke(section, new[] { value });
                }
            }

            return new Options<T> {Value = section};
        }
    }
}