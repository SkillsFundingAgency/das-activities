using System;
using System.Linq;
using SFA.DAS.Activities.Worker.Configuration;
using StructureMap;
using StructureMap.Pipeline;
using SFA.DAS.Configuration;

namespace SFA.DAS.Activities.WorkerRole.Configuration.Policies
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