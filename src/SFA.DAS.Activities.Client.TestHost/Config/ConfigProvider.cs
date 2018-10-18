﻿using System;
using System.Collections.Concurrent;
using System.IO;
using Newtonsoft.Json.Linq;
using SFA.DAS.Activities.Client.TestHost.Interfaces;

namespace SFA.DAS.Activities.Client.TestHost.Config
{
    public class ConfigProvider : IConfigProvider
    {
        private readonly ConcurrentDictionary<Type, object> _config = new ConcurrentDictionary<Type, object>();
        private readonly Lazy<JObject> _fullConfigFile = new Lazy<JObject>(LoadConfigFile);
        private const string ConfigFileName = "SFA.DAS.Activities.Client.TestHost.json";

        public TConfigType Get<TConfigType>() where TConfigType : class, new()
        {
            return _config.GetOrAdd(typeof(TConfigType), LoadConfig) as TConfigType;
        }

        private object LoadConfig(Type configType)
        {
            return _fullConfigFile.Value.Value<JObject>(configType.Name).ToObject(configType);
        }

        private static JObject LoadConfigFile()
        {
            if (!File.Exists(ConfigFileName))
            {
                throw new FileNotFoundException("The config file required for type {configType.Name} does not exist", ConfigFileName);
            }

            return JObject.Parse(File.ReadAllText(ConfigFileName));
        }
    }
}