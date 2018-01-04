using System;
using System.IO;
using System.Linq;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.AzureServiceBus.Helpers;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.Activities.Worker.Policies
{
    public class MessageSubscriberPolicy<T> : ConfiguredInstancePolicy where T : IMessageServiceBusConfiguration
    {
        private readonly string _serviceName;
        private readonly string _version;
        private readonly ILog _logger;

        public MessageSubscriberPolicy(string serviceName, string version, ILog logger)
        {
            _serviceName = serviceName;
            _version = version;
            _logger = logger;
        }

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var factoryParameter = instance?.Constructor?.GetParameters().FirstOrDefault(p => p.ParameterType == typeof(IMessageSubscriberFactory));

            if (factoryParameter == null)
            {
                return;
            }

            var config = ConfigurationHelper.GetConfiguration<T>(_serviceName, _version);

            if (string.IsNullOrEmpty(config.MessageServiceBusConnectionString))
            {
                var groupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _serviceName);
                var factory = new FileSystemMessageSubscriberFactory(groupFolder);

                instance.Dependencies.AddForConstructorParameter(factoryParameter, factory);
            }
            else
            {
                var subscriptionName = TopicSubscriptionHelper.GetMessageGroupName(instance.Constructor.DeclaringType);
                var factory = new TopicSubscriberFactory(config.MessageServiceBusConnectionString, subscriptionName, _logger);

                instance.Dependencies.AddForConstructorParameter(factoryParameter, factory);
            }
        }
    }
}