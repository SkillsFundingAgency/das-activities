using System;
using System.Collections.Generic;

namespace SFA.DAS.Activities.Configuration
{
    public class SettingsBuilder : ISettingsBuilder
    {
        private readonly ICollection<IProvideSettings> _providers = new List<IProvideSettings>();

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
                        Console.WriteLine($"Loaded {provider.GetType().Name} > [{pair.Key}]");
                        values.Add(pair.Key, pair.Value);
                    }
                }
            }

            return new SettingsProvider(values);
        }
    }
}