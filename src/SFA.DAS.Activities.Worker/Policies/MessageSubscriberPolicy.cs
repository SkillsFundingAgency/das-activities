using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.AzureServiceBus.Helpers;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.Messaging.Interfaces;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.Activities.Worker.Policies
{
    public class MessageSubscriberPolicy : ConfiguredInstancePolicy
    {
        private readonly Func<IServiceBusConfiguration> _configuration;
        private readonly string _serviceName;

        public MessageSubscriberPolicy(Func<IServiceBusConfiguration> configuration, string serviceName)
        {
            _configuration = configuration;
            _serviceName = serviceName;
        }

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var factoryParameter = instance?.Constructor?.GetParameters().FirstOrDefault(p => p.ParameterType == typeof(IMessageSubscriberFactory));

            if (factoryParameter == null)
            {
                return;
            }

            if (Debugger.IsAttached)
            {
                var groupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _serviceName);
                var factory = new FileSystemMessageSubscriberFactory(groupFolder);

                if (!Directory.Exists(groupFolder))
                {
                    Directory.CreateDirectory(groupFolder);
                }

                instance.Dependencies.AddForConstructorParameter(factoryParameter, factory);
            }
            else
            {
                var subscriptionName = TopicSubscriptionHelper.GetMessageGroupName(instance.Constructor.DeclaringType);
                var factory = new TopicSubscriberFactory(_configuration().ConnectionString, subscriptionName);

                instance.Dependencies.AddForConstructorParameter(factoryParameter, factory);
            }
        }
    }
}