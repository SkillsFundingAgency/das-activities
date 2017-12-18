using System;
using System.Collections.Generic;

namespace SFA.DAS.Activities.Configuration
{
    public class SettingsBuilder : ISettingsBuilder
    {
        private readonly ICollection<ISettingsProvider> _providers = new List<ISettingsProvider>();

        public SettingsBuilder AddProvider(ISettingsProvider provider)
        {
            _providers.Add(provider);
            return this;
        }

        public Settings Build()
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

            return new Settings(values);
        }
    }
}