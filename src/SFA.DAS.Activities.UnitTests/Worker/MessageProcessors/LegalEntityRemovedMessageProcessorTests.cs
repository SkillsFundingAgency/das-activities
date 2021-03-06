﻿using NUnit.Framework;
using SFA.DAS.Activities.MessageHandlers.MessageProcessors;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.Activities.UnitTests.Worker.MessageProcessors
{
    public class LegalEntityRemovedMessageProcessorTests : MessageProcessorTest<LegalEntityRemovedMessageProcessor>
    {
        [Test]
        public void When_processing_LegalEntityRemovedMessage_then_should_index_LegalEntityRemoved_activity()
        {
            From(new LegalEntityRemovedMessage()).To(ActivityType.LegalEntityRemoved).Verify(CreateMessageProcessor);
        }

        private LegalEntityRemovedMessageProcessor CreateMessageProcessor(
            MessageProcessorTestFixtures<LegalEntityRemovedMessage> fixtures)
        {
            return new LegalEntityRemovedMessageProcessor(fixtures.SubscriberFactory, fixtures.Log, fixtures.ActivitySaver, fixtures.MessageContextProvider);
        }
    }
}