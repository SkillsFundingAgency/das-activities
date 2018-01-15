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
    [TopicSubscription("Activity_AgreementSignedMessageProcessor")]
    public class AgreementSignedMessageProcessor : MessageProcessor<AgreementSignedMessage>
    {
        private readonly IActivityMapper _activityMapper;
        private readonly IElasticClient _client;

        public AgreementSignedMessageProcessor(IMessageSubscriberFactory subscriberFactory, ILog logger, IActivityMapper activityMapper, IElasticClient client)
            : base(subscriberFactory, logger)
        {
            _activityMapper = activityMapper;
            _client = client;
        }

        protected override async Task ProcessMessage(AgreementSignedMessage message)
        {
            var activity = _activityMapper.Map(message, ActivityType.AgreementSigned);
            await _client.IndexAsync(activity);
        }
    }
}