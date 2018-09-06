using NUnit.Framework;
using SFA.DAS.Activities.MessageHandlers.MessageProcessors;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.Activities.UnitTests.Worker.MessageProcessors
{
    public class LegalEntityAddedMessageProcessorTests : MessageProcessorTest<LegalEntityAddedMessageProcessor>
    {
        [Test]
        public void When_processing_LegalEntityAddedMessage_then_should_index_LegalEntityAdded_activity()
        {
            From(new LegalEntityAddedMessage()).To(ActivityType.LegalEntityAdded).Verify(CreateMessageProcessor);
        }

        private LegalEntityAddedMessageProcessor CreateMessageProcessor(MessageProcessorTestFixtures<LegalEntityAddedMessage> fixtures)
        {
            return new LegalEntityAddedMessageProcessor(fixtures.SubscriberFactory, fixtures.Log, fixtures.ActivitySaver, fixtures.MessageContextProvider);
        }
    }
}