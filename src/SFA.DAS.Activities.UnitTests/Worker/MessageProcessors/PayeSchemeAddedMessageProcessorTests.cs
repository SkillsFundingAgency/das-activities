﻿using NUnit.Framework;
using SFA.DAS.Activities.MessageHandlers.MessageProcessors;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.Activities.UnitTests.Worker.MessageProcessors
{
    public class PayeSchemeAddedMessageProcessorTests : MessageProcessorTest<PayeSchemeAddedMessageProcessor>
    {
        [Test]
        public void When_processing_PayeSchemeAddedMessage_then_should_index_PayeSchemeAdded_activity()
        {
            From(new PayeSchemeAddedMessage()).To(ActivityType.PayeSchemeAdded).Verify(CreateMessageProcessor);
        }

        private PayeSchemeAddedMessageProcessor CreateMessageProcessor(
            MessageProcessorTestFixtures<PayeSchemeAddedMessage> fixtures)
        {
            return new PayeSchemeAddedMessageProcessor(fixtures.SubscriberFactory, fixtures.Log, fixtures.ActivitySaver, fixtures.MessageContextProvider);
        }
    }
}