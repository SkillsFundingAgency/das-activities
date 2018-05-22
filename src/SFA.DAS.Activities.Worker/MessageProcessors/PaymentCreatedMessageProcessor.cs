using System.Threading.Tasks;
using Nest;
using SFA.DAS.Activities.Worker.ActivitySavers;
using SFA.DAS.Activities.Worker.ObjectMappers;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.Worker.MessageProcessors
{
    [TopicSubscription("Activity_PaymentCreatedMessageProcessor")]
    public class PaymentCreatedMessageProcessor : MessageProcessor<PaymentCreatedMessage>
    {
        private readonly IActivitySaver _activitySaver;

        public PaymentCreatedMessageProcessor(
            IMessageSubscriberFactory subscriberFactory,
            ILog log,
            IActivitySaver activitySaver,
            IMessageContextProvider messageContextProvider)
            : base(subscriberFactory, log, messageContextProvider)
        {
            _activitySaver = activitySaver;
        }

        protected override Task ProcessMessage(PaymentCreatedMessage message)
        {
            return _activitySaver.SaveActivity(message, ActivityType.PaymentCreated);
        }
    }
}
