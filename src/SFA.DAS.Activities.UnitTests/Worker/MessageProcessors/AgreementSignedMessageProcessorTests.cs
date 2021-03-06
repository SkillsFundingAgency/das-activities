﻿using NUnit.Framework;
using SFA.DAS.Activities.MessageHandlers.MessageProcessors;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.Activities.UnitTests.Worker.MessageProcessors
{
    public class AgreementSignedMessageProcessorTests : MessageProcessorTest<AgreementSignedMessageProcessor>
    {
        [Test]
        public void When_processing_PayeSchemeAddedMessage_then_should_index_PayeSchemeAdded_activity()
        {
            From(new AgreementSignedMessage()).To(ActivityType.AgreementSigned).Verify(CreateMessageProcessor);
        }

        private AgreementSignedMessageProcessor CreateMessageProcessor(
            MessageProcessorTestFixtures<AgreementSignedMessage> fixtures)
        {
            return new AgreementSignedMessageProcessor(fixtures.SubscriberFactory, fixtures.Log, fixtures.ActivitySaver, fixtures.MessageContextProvider);
        }
    }
}