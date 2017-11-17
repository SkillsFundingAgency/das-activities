using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SFA.DAS.Activities.Application.Configurations;

namespace SFA.DAS.Activities.WebJob.Configuration
{
    public sealed class MachineSettings : IProvideSettings
    {
        private readonly string _prefix;

        public MachineSettings(string prefix)
        {
            _prefix = prefix;
        }

        public string GetSetting(string settingKey)
        {
            return Environment.GetEnvironmentVariable($"{GetKey(settingKey)}", EnvironmentVariableTarget.User) ??
                Environment.GetEnvironmentVariable($"{settingKey}", EnvironmentVariableTarget.User);
        }

        public string GetNullableSetting(string settingKey)
        {
            return GetSetting(settingKey);
        }

        public IEnumerable<string> GetArray(string settingKey)
        {
            var collection = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User).Keys;
            var enumerable = collection.Cast<string>();
            var arrayKeys = enumerable.Where(x => x.StartsWith($"{settingKey}:")).OrderBy(x => x);
            foreach (var key in arrayKeys)
            {
                yield return Environment.GetEnvironmentVariable(GetKey(key), EnvironmentVariableTarget.User);
            }
        }

        private string GetKey(string settingKey)
        {
            return $"{_prefix}{settingKey.ToUpper(CultureInfo.InvariantCulture)}";
        }
    }
}
