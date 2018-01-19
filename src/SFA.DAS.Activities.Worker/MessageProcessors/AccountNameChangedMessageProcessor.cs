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
    [TopicSubscription("Activity_AccountNameChangedMessageProcessor")]
    public class AccountNameChangedMessageProcessor : MessageProcessor<AccountNameChangedMessage>
    {
        private readonly IActivityMapper _activityMapper;
        private readonly IElasticClient _client;

        public AccountNameChangedMessageProcessor(
            IMessageSubscriberFactory subscriberFactory, 
            ILog log, 
            IActivityMapper activityMapper, 
            IElasticClient client) 
            : base(subscriberFactory, log)
        {
            _activityMapper = activityMapper;
            _client = client;
        }

        protected override async Task ProcessMessage(AccountNameChangedMessage message)
        {
            var activity = _activityMapper.Map(message, ActivityType.AccountNameChanged);
            await _client.IndexAsync(activity);
        }
    }
}
