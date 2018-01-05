using NUnit.Framework;
using SFA.DAS.Activities.Worker.MessageProcessors;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.Activities.UnitTests.Worker.MessageProcessors
{
    public class LegalEntityAddedMessageProcessorTests : MessageProcessorTest<LegalEntityAddedMessageProcessor>
    {
        [Test]
        public void When_processing_LegalEntityAddedMessage_then_should_index_LegalEntityAdded_activity()
        {
            From(new LegalEntityAddedMessage()).To(ActivityType.LegalEntityAdded).Verify();
        }
    }
}