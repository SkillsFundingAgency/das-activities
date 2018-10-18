using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace PerformanceTester.Types
{
    public class ConfigProvider : IConfigProvider
    {
        private readonly ConcurrentDictionary<Type, object> _config = new ConcurrentDictionary<Type, object>();
        private readonly Lazy<JObject> _fullConfigFile = new Lazy<JObject>(ConfigProvider.LoadConfigFile);
        private const string ConfigFileName = "PerformanceTester.json";

        public TConfigType Get<TConfigType>() where TConfigType : class, new()
        {
            return _config.GetOrAdd(typeof(TConfigType), new Func<Type, object>(LoadConfig)) as TConfigType;
        }

        private object LoadConfig(Type configType)
        {
            return _fullConfigFile.Value.Value<JObject>((object)configType.Name).ToObject(configType);
        }

        private static JObject LoadConfigFile()
        {
            if (!File.Exists(ConfigFileName))
            {
                throw new FileNotFoundException("The config file required for type {configType.Name} does not exist", ConfigFileName);
            }

            return JObject.Parse(File.ReadAllText("PerformanceTester.json"));
        }
    }
}
