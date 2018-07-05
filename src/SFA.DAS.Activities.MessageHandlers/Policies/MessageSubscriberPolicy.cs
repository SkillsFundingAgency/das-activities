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

namespace SFA.DAS.Activities.MessageHandlers.Policies
{
    public class MessageSubscriberPolicy : ConfiguredInstancePolicy
    {
        private readonly string _serviceName;
        private readonly IMessageServiceBusConfiguration _config;
        private readonly ILog _logger;

        public MessageSubscriberPolicy(string serviceName, IMessageServiceBusConfiguration config, ILog logger)
        {
            _serviceName = serviceName;
            _config = config;
            _logger = logger;
        }

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var factoryParameter = instance?.Constructor?.GetParameters().FirstOrDefault(p => p.ParameterType == typeof(IMessageSubscriberFactory));

            if (factoryParameter == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(_config.MessageServiceBusConnectionString))
            {
                var groupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _serviceName);
                var factory = new FileSystemMessageSubscriberFactory(groupFolder);

                instance.Dependencies.AddForConstructorParameter(factoryParameter, factory);
            }
            else
            {
                var subscriptionName = TopicSubscriptionHelper.GetMessageGroupName(instance.Constructor.DeclaringType);
                var factory = new TopicSubscriberFactory(_config.MessageServiceBusConnectionString, subscriptionName, _logger);

                instance.Dependencies.AddForConstructorParameter(factoryParameter, factory);
            }
        }
    }
}