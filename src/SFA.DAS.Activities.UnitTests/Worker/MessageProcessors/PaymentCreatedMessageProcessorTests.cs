using NUnit.Framework;
using SFA.DAS.Activities.Worker.MessageProcessors;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.Activities.UnitTests.Worker.MessageProcessors
{
    public class PaymentCreatedMessageProcessorTests : MessageProcessorTest<PaymentCreatedMessageProcessor>
    {
        [Test]
        public void When_processing_PaymentCreatedMessage_then_should_index_PaymentCreated_activity()
        {
            From(new PaymentCreatedMessage()).To(ActivityType.PaymentCreated).Verify();
        }
    }
}