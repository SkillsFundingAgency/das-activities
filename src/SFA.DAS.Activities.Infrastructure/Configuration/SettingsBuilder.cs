using System.Collections.Generic;
using SFA.DAS.Activities.Application.Configurations;

namespace SFA.DAS.Activities.Infrastructure.Configuration
{
    public class SettingsBuilder : ISettingsBuilder
    {
        private ICollection<IProvideSettings> _providers = new List<IProvideSettings>();

        public SettingsBuilder AddProvider(IProvideSettings provider)
        {
            _providers.Add(provider);
            return this;
        }

        public SettingsProvider Build()
        {
            var values = new Dictionary<string, object>();
            foreach (var provider in _providers)
            {
                foreach (var pair in provider.Settings)
                {
                    if (!values.ContainsKey(pair.Key))
                    {
                        values.Add(pair.Key, pair.Value);
                    }
                }
            }

            return new SettingsProvider(values);
        }
    }
}