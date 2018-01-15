using NUnit.Framework;
using SFA.DAS.Activities.Worker.MessageProcessors;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.Activities.UnitTests.Worker.MessageProcessors
{
    public class AgreementSignedMessageProcessorTests : MessageProcessorTest<AgreementSignedMessageProcessor>
    {
        [Test]
        public void When_processing_PayeSchemeAddedMessage_then_should_index_PayeSchemeAdded_activity()
        {
            From(new AgreementSignedMessage()).To(ActivityType.AgreementSigned).Verify();
        }
    }
}