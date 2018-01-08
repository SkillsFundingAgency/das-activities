using NUnit.Framework;
using SFA.DAS.Activities.Worker.MessageProcessors;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.Activities.UnitTests.Worker.MessageProcessors
{
    public class AccountNameChangedMessageProcessorTests : MessageProcessorTest<AccountNameChangedMessageProcessor>
    {
        [Test]
        public void When_processing_AccountCreatedMessage_then_should_index_AccountCreated_activity()
        {
            From(new AccountNameChangedMessage()).To(ActivityType.AccountNameChanged).Verify();
        }
    }
}