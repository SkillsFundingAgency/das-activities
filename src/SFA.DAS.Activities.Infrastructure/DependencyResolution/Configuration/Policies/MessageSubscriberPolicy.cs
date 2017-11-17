//using System;
//using System.Linq;
//using System.Reflection;
//using SFA.DAS.Messaging.AzureServiceBus;
//using SFA.DAS.Messaging.AzureServiceBus.Helpers;
//using SFA.DAS.Messaging.FileSystem;
//using SFA.DAS.Messaging.Interfaces;
//using StructureMap;
//using StructureMap.Pipeline;
//using SFA.DAS.Activities.Application.Configurations;

//namespace SFA.DAS.Activities.Infrastructure.DependencyResolution.Configuration.Policies
//{
//    public class MessageSubscriberPolicy : ConfiguredInstancePolicy
//    {
//        private readonly IActivitiesConfiguration _settings;

//        public MessageSubscriberPolicy()
//        {
//            var provider = new AppConfigSettingsProvider(new MachineSettings("Activities:"));
//            _settings = new ActivitiesConfiguration(provider);
//        }

//        protected override void apply(Type pluginType, IConfiguredInstance instance)
//        {
//            var subscriberFactory = GetMessageSubscriberFactoryParameter(instance);

//            if (subscriberFactory == null) return;


//            if (string.IsNullOrEmpty(_settings.ServiceBusConnectionString))
//            {
//                var groupFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/EAS_Queues/";
//                var factory = new FileSystemMessageSubscriberFactory(groupFolder);

//                instance.Dependencies.AddForConstructorParameter(subscriberFactory, factory);
//            }
//            else
//            {
//                var subscriptionName = TopicSubscriptionHelper.GetMessageGroupName(instance.Constructor.DeclaringType);

//                var factory = new TopicSubscriberFactory(_settings.ServiceBusConnectionString, subscriptionName);

//                instance.Dependencies.AddForConstructorParameter(subscriberFactory, factory);
//            }
//        }

//        private static ParameterInfo GetMessageSubscriberFactoryParameter(IConfiguredInstance instance)
//        {
//            var factory = instance?.Constructor?
//                .GetParameters().FirstOrDefault(x => x.ParameterType == typeof(IMessageSubscriberFactory));

//            return factory;
//        }
//    }
//}