using NUnit.Framework;
using SFA.DAS.Activities.Worker.MessageProcessors;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.Activities.UnitTests.Worker.MessageProcessors
{
    public class PayeSchemeRemovedMessageProcessorTests : MessageProcessorTest<PayeSchemeRemovedMessageProcessor>
    {
        [Test]
        public void When_processing_PayeSchemeRemovedMessage_then_should_index_PayeSchemeRemoved_activity()
        {
            From(new PayeSchemeDeletedMessage()).To(ActivityType.PayeSchemeRemoved).Verify();
        }
    }
}