﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Activities.Worker.MessageProcessors;
using SFA.DAS.Activities.Worker.ObjectMappers;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.UnitTests.Worker.MessageProcessors
{
    public static class LegalEntityAddedMessageProcessorTests
    {
        public class When_processing_LegalEntityAddedMessage : TestAsync
        {
            private LegalEntityAddedMessageProcessor _messageProcessor;
            private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
            private readonly Mock<IMessageSubscriberFactory> _subscriberFactory = new Mock<IMessageSubscriberFactory>();
            private readonly Mock<IMessageSubscriber<LegalEntityAddedMessage>> _subscriber = new Mock<IMessageSubscriber<LegalEntityAddedMessage>>();
            private readonly Mock<IMessage<LegalEntityAddedMessage>> _logicalMessage = new Mock<IMessage<LegalEntityAddedMessage>>();
            private readonly LegalEntityAddedMessage _message = new LegalEntityAddedMessage();
            private readonly Mock<IActivityMapper> _activityMapper = new Mock<IActivityMapper>();
            private readonly Mock<IElasticClient> _client = new Mock<IElasticClient>();
            private readonly Activity _mappedActivity = new Activity();
            private Activity _activity;

            protected override void Given()
            {
                _logicalMessage.Setup(m => m.Content).Returns(_message);
                _subscriber.Setup(s => s.ReceiveAsAsync()).ReturnsAsync(_logicalMessage.Object).Callback(_cancellationTokenSource.Cancel);
                _subscriberFactory.Setup(s => s.GetSubscriber<LegalEntityAddedMessage>()).Returns(_subscriber.Object);

                _activityMapper.Setup(m => m.Map(_message, ActivityType.LegalEntityAdded, null, null))
                    .Returns(_mappedActivity);

                _client.Setup(c => c.IndexAsync(_mappedActivity, It.IsAny<Func<IndexDescriptor<Activity>, IIndexRequest>>(), It.IsAny<CancellationToken>()))
                    .Callback<Activity, Func<IndexDescriptor<Activity>, IIndexRequest>, CancellationToken>((a, s, c) => _activity = a);

                _messageProcessor = new LegalEntityAddedMessageProcessor(_subscriberFactory.Object, Mock.Of<ILog>(), _activityMapper.Object, _client.Object);
            }

            protected override async Task When()
            {
                await _messageProcessor.RunAsync(_cancellationTokenSource.Token);
            }

            [Test]
            public void Then_should_index_activity()
            {
                Assert.That(_activity, Is.Not.Null);
                Assert.That(_activity, Is.SameAs(_mappedActivity));
            }
        }
    }
}