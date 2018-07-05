using NUnit.Framework;
using SFA.DAS.Activities.MessageHandlers.MessageProcessors;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.Activities.UnitTests.Worker.MessageProcessors
{
    public class PaymentCreatedMessageProcessorTests : MessageProcessorTest<PaymentCreatedMessageProcessor>
    {
        [Test]
        public void When_processing_PaymentCreatedMessage_then_should_index_PaymentCreated_activity()
        {
            From(new PaymentCreatedMessage()).To(ActivityType.PaymentCreated).Verify(CreateMessageProcessor);
        }

        private PaymentCreatedMessageProcessor CreateMessageProcessor(
            MessageProcessorTestFixtures<PaymentCreatedMessage> fixtures)
        {
            return new PaymentCreatedMessageProcessor(fixtures.SubscriberFactory, fixtures.Log, fixtures.ActivitySaver, fixtures.MessageContextProvider);
        }
    }
}