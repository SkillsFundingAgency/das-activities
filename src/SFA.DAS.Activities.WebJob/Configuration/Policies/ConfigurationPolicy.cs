using System;
using System.Linq;
using SFA.DAS.Configuration;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.Activities.WebJob.Configuration.Policies
{

    public class ConfigurationPolicy<T> : ConfiguredInstancePolicy where T : IConfiguration
    {
        private readonly string _serviceName;
        private readonly IConfigurationInfo<T> _configInfo;

        public ConfigurationPolicy(string serviceName)
        {
            _serviceName = serviceName;
            _configInfo = null;
        }

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var serviceConfigurationParamater = instance?.Constructor?.GetParameters().FirstOrDefault(x => x.ParameterType == typeof(T)
                                                                                                           || ((System.Reflection.TypeInfo)typeof(T)).GetInterface(x.ParameterType.Name) != null);

            if (serviceConfigurationParamater != null)
            {
                var result = _configInfo.GetConfiguration(_serviceName);
                if (result != null)
                {
                    instance.Dependencies.AddForConstructorParameter(serviceConfigurationParamater, result);
                }
            }

        }
    }
}