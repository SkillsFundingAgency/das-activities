//using System;
//using System.Linq;
//using System.Reflection;
//using Microsoft.WindowsAzure;
//using SFA.DAS.Messaging.AzureServiceBus;
//using SFA.DAS.Messaging.FileSystem;
//using SFA.DAS.Messaging.Interfaces;
//using StructureMap;
//using StructureMap.Pipeline;
//using System.Configuration;
//using Microsoft.Azure;
//using SFA.DAS.Activities.Application.Configurations;

//namespace SFA.DAS.Activities.Infrastructure.DependencyResolution.Configuration.Policies
//{
//    public class MessagePublisherPolicy : ConfiguredInstancePolicy
//    {
//        private readonly IActivitiesConfiguration _settings;

//        public MessagePublisherPolicy(IActivitiesConfiguration settings)
//        {
//            _settings = settings;
//        }

//        protected override void apply(Type pluginType, IConfiguredInstance instance)
//        {
//            var messagePublisher = GetMessagePublisherParameter(instance);

//            if (messagePublisher == null) return;


//            if (string.IsNullOrEmpty(_settings.ServiceBusConnectionString))
//            {
//                var groupFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/EAS_Queues/";

//                var publisher = new FileSystemMessagePublisher(groupFolder);
//                instance.Dependencies.AddForConstructorParameter(messagePublisher, publisher);
//            }
//            else
//            {
//                var publisher = new TopicMessagePublisher(_settings.ServiceBusConnectionString);
//                instance.Dependencies.AddForConstructorParameter(messagePublisher, publisher);
//            }
//        }

//        private static ParameterInfo GetMessagePublisherParameter(IConfiguredInstance instance)
//        {
//            var messagePublisher = instance?.Constructor?
//                .GetParameters().FirstOrDefault(x => x.ParameterType == typeof(IMessagePublisher));
//            return messagePublisher;
//        }
//    }
//}