﻿using System;
using System.IO;
using System.Linq;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.Attributes;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.Messaging.AzureServiceBus;
using StructureMap;
using StructureMap.Pipeline;
using System.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Activities.Worker.Configuration.Policies
{
    namespace SFA.DAS.EAS.Infrastructure.DependencyResolution
    {
        public class MessagePolicy<T> : ConfiguredInstancePolicy where T : IConfiguration
        {
            private readonly string _serviceName;
            private readonly IConfigurationInfo<T> _configInfo;

            public MessagePolicy(string serviceName)
            {
                _serviceName = serviceName;
                _configInfo = null;
            }

            protected override void apply(Type pluginType, IConfiguredInstance instance)
            {
                var messagePublisher = instance?.Constructor?
                    .GetParameters().FirstOrDefault(x => x.ParameterType == typeof(IMessagePublisher) || x.ParameterType == typeof(IMessageSubscriberFactory));

                var environment = Environment.GetEnvironmentVariable("DASENV");
                if (string.IsNullOrEmpty(environment))
                {
                    environment = CloudConfigurationManager.GetSetting("EnvironmentName");
                }

                if (messagePublisher != null)
                {
                    var queueName = instance.SettableProperties()
                        .FirstOrDefault(c => c.CustomAttributes.FirstOrDefault(
                                                 x => x.AttributeType.Name == nameof(QueueNameAttribute)) != null);


                    var configurationService = new ConfigurationService(GetConfigurationRepository(), new ConfigurationOptions(_serviceName, environment, "1.0"));

                    var config = configurationService.Get<T>();
                    if (string.IsNullOrEmpty(config.ServiceBusConnectionString))
                    {
                        var queueFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        instance.Dependencies.AddForConstructorParameter(messagePublisher, new FileSystemMessageService(Path.Combine(queueFolder, queueName?.Name ?? string.Empty)));
                    }
                    else
                    {
                        instance.Dependencies.AddForConstructorParameter(messagePublisher, new AzureServiceBusMessageService(config.ServiceBusConnectionString, queueName?.Name ?? string.Empty));
                    }
                }
            }

            private static IConfigurationRepository GetConfigurationRepository()
            {
                IConfigurationRepository configurationRepository;
                if (bool.Parse(ConfigurationManager.AppSettings["LocalConfig"]))
                {
                    configurationRepository = new FileStorageConfigurationRepository();
                }
                else
                {
                    configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
                }
                return configurationRepository;
            }
        }
    }
}