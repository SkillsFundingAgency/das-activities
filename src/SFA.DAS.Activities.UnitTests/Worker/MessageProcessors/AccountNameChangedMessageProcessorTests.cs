﻿using NUnit.Framework;
using SFA.DAS.Activities.Worker.MessageProcessors;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.Activities.UnitTests.Worker.MessageProcessors
{
    public class AccountNameChangedMessageProcessorTests : MessageProcessorTest<AccountNameChangedMessageProcessor>
    {
        [Test]
        public void When_processing_AccountNameChangedMessage_then_should_index_AccountNameChanged_activity()
        {
            From(new AccountNameChangedMessage()).To(ActivityType.AccountNameChanged).Verify();
        }
    }
}