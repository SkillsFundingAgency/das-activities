using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.MessageHandlers.MessageProcessors
{
    [TopicSubscription("Activity_PayeSchemeAddedMessageProcessor")]
    public class PayeSchemeAddedMessageProcessor : MessageProcessor<PayeSchemeAddedMessage>
    {
        private readonly IActivitySaver _activitySaver;

        public PayeSchemeAddedMessageProcessor(IMessageSubscriberFactory subscriberFactory,
            ILog log,
            IActivitySaver activitySaver,
            IMessageContextProvider messageContextProvider)
            : base(subscriberFactory, log, messageContextProvider)
        {
            _activitySaver = activitySaver;
        }

        protected override Task ProcessMessage(PayeSchemeAddedMessage message)
        {
            return _activitySaver.SaveActivity(message, ActivityType.PayeSchemeAdded);
        }
    }
}