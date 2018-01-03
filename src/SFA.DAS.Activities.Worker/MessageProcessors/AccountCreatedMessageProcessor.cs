using System.Threading.Tasks;
using Nest;
using SFA.DAS.Activities.Worker.ObjectMappers;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.Worker.MessageProcessors
{
    public class AccountCreatedMessageProcessor : MessageProcessor<AccountCreatedMessage>
    {
        private readonly IActivityMapper _activityMapper;
        private readonly IElasticClient _client;

        public AccountCreatedMessageProcessor(
            IMessageSubscriberFactory subscriberFactory, 
            ILog log, 
            IActivityMapper activityMapper, 
            IElasticClient client) 
            : base(subscriberFactory, log)
        {
            _activityMapper = activityMapper;
            _client = client;
        }

        protected override async Task ProcessMessage(AccountCreatedMessage message)
        {
            var activity = _activityMapper.Map(message, ActivityType.AccountCreated);
            await _client.IndexAsync(activity);
        }
    }
}
