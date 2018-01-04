using System.Threading.Tasks;
using Nest;
using SFA.DAS.Activities.Worker.ObjectMappers;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.Worker.MessageProcessors
{
    [TopicSubscription("Activity_LegalEntityRemovedMessageProcessor")]
    public class LegalEntityRemovedMessageProcessor : MessageProcessor<LegalEntityRemovedMessage>
    {
        private readonly IActivityMapper _activityMapper;
        private readonly IElasticClient _client;

        public LegalEntityRemovedMessageProcessor(
            IMessageSubscriberFactory subscriberFactory, 
            ILog log, 
            IActivityMapper activityMapper, 
            IElasticClient client) 
            : base(subscriberFactory, log)
        {
            _activityMapper = activityMapper;
            _client = client;
        }

        protected override async Task ProcessMessage(LegalEntityRemovedMessage message)
        {
            var activity = _activityMapper.Map(message, ActivityType.AccountCreated);
            await _client.IndexAsync(activity);
        }
    }
}
