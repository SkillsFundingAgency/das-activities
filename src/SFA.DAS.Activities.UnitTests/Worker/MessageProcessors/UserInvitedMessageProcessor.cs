﻿using NUnit.Framework;
using SFA.DAS.Activities.MessageHandlers.MessageProcessors;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.Activities.UnitTests.Worker.MessageProcessors
{
    public class UserInvitedMessageProcessorTests : MessageProcessorTest<UserInvitedMessageProcessor>
    {
        [Test]
        public void When_processing_PayeSchemeAddedMessage_then_should_index_PayeSchemeAdded_activity()
        {
            From(new UserInvitedMessage()).To(ActivityType.UserInvited).Verify(CreateMessageProcessor);
        }

        private UserInvitedMessageProcessor CreateMessageProcessor(
            MessageProcessorTestFixtures<UserInvitedMessage> fixtures)
        {
            return new UserInvitedMessageProcessor(fixtures.SubscriberFactory, fixtures.Log, fixtures.ActivitySaver, fixtures.MessageContextProvider);
        }
    }
}