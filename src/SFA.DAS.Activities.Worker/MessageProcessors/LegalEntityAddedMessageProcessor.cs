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
    [TopicSubscription("Activity_LegalEntityAddedMessageProcessor")]
    public class LegalEntityAddedMessageProcessor : MessageProcessor<LegalEntityAddedMessage>
    {
        private readonly IActivityMapper _activityMapper;
        private readonly IElasticClient _client;

        public LegalEntityAddedMessageProcessor(
            IMessageSubscriberFactory subscriberFactory, 
            ILog log, 
            IActivityMapper activityMapper, 
            IElasticClient client) 
            : base(subscriberFactory, log)
        {
            _activityMapper = activityMapper;
            _client = client;
        }

        protected override async Task ProcessMessage(LegalEntityAddedMessage message)
        {
            var activity = _activityMapper.Map(message, ActivityType.LegalEntityAdded);
            await _client.IndexAsync(activity);
        }
    }
}
