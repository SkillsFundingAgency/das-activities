﻿using NUnit.Framework;
using SFA.DAS.Activities.MessageHandlers.MessageProcessors;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.Activities.UnitTests.Worker.MessageProcessors
{
    public class UserJoinedMessageProcessorTests : MessageProcessorTest<UserJoinedMessageProcessor>
    {
        [Test]
        public void When_processing_UserJoinedMessage_then_should_index_UserJoined_activity()
        {
            From(new UserJoinedMessage()).To(ActivityType.UserJoined).Verify(CreateMessageProcessor);
        }

        private UserJoinedMessageProcessor CreateMessageProcessor(
            MessageProcessorTestFixtures<UserJoinedMessage> fixtures)
        {
            return new UserJoinedMessageProcessor(fixtures.SubscriberFactory, fixtures.Log, fixtures.ActivitySaver, fixtures.MessageContextProvider);
        }
    }
}