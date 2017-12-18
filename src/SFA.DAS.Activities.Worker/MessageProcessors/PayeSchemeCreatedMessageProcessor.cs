using System.Threading.Tasks;
using SFA.DAS.Activities.Worker.ObjectMappers;
using SFA.DAS.Activities.Worker.Services;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.Worker.MessageProcessors
{
    [TopicSubscription("Activity_PayeSchemeCreatedMessageProcessor")]
    public class PayeSchemeCreatedMessageProcessor : MessageProcessor<PayeSchemeCreatedMessage>
    {
        private readonly IMessageSubscriberFactory _subscriberFactory;
        private readonly IActivityMapper _activityMapper;
        private readonly IActivitiesService _activitiesService;

        public PayeSchemeCreatedMessageProcessor(IMessageSubscriberFactory subscriberFactory, ILog logger, IActivityMapper activityMapper, IActivitiesService activitiesService)
            : base(subscriberFactory, logger)
        {
            _subscriberFactory = subscriberFactory;
            _activityMapper = activityMapper;
            _activitiesService = activitiesService;
        }

        protected override async Task ProcessMessage(PayeSchemeCreatedMessage message)
        {
            await _activitiesService.AddActivity(_activityMapper.Map(message, ActivityType.PayeSchemeAdded));
        }
    }
}