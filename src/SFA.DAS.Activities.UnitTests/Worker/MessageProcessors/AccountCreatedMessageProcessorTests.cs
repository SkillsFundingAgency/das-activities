using NUnit.Framework;
using SFA.DAS.Activities.MessageHandlers.MessageProcessors;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.Activities.UnitTests.Worker.MessageProcessors
{
    public class AccountCreatedMessageProcessorTests : MessageProcessorTest<AccountCreatedMessageProcessor>
    {
        [Test]
        public void When_processing_AccountCreatedMessage_then_should_index_AccountCreated_activity()
        {
            From(new AccountCreatedMessage()).To(ActivityType.AccountCreated).Verify(CreateMessageProcessor);
        }

        private AccountCreatedMessageProcessor CreateMessageProcessor(
            MessageProcessorTestFixtures<AccountCreatedMessage> fixtures)
        {
            return new AccountCreatedMessageProcessor(fixtures.SubscriberFactory, fixtures.Log, fixtures.ActivitySaver, fixtures.MessageContextProvider);
        }

    }
}