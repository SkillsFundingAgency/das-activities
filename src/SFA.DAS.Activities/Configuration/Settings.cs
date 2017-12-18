using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Activities.Configuration
{
    public class Settings : ISettings
    {
        private readonly Dictionary<string, object> _values;

        public Settings(Dictionary<string, object> values)
        {
            _values = values;
        }

        public T GetSection<T>(string name) where T : new()
        {
            var section = new T();

            foreach (var propertyInfo in typeof(T).GetProperties().Where(x => x.CanWrite))
            {
                var setter = propertyInfo.GetSetMethod();
                var key = $"{name}:{propertyInfo.Name}";

                if (_values.ContainsKey(key) && setter != null)
                {
                    var value = _values[key];
                    setter.Invoke(section, new[] { value });
                }
            }

            return section;
        }
    }
}