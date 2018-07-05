using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.MessageHandlers.MessageProcessors
{
    [TopicSubscription("Activity_LegalEntityAddedMessageProcessor")]
    public class LegalEntityAddedMessageProcessor : MessageProcessor<LegalEntityAddedMessage>
    {
        private readonly IActivitySaver _activitySaver;

        public LegalEntityAddedMessageProcessor(
            IMessageSubscriberFactory subscriberFactory,
            ILog log,
            IActivitySaver activitySaver,
            IMessageContextProvider messageContextProvider)
            : base(subscriberFactory, log, messageContextProvider)
        {
            _activitySaver = activitySaver;
        }

        protected override Task ProcessMessage(LegalEntityAddedMessage message)
        {
            return _activitySaver.SaveActivity(message, ActivityType.LegalEntityAdded);
        }
    }
}
