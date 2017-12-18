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

namespace SFA.DAS.Activities.UnitTests.Worker
{
    public static class PayeSchemeCreatedMessageProcessorTests
    {
        public class When_processing_PayeSchemeCreatedMessage: TestAsync
        {
            private PayeSchemeCreatedMessageProcessor _messageProcessor;
            private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
            private readonly Mock<IMessageSubscriberFactory> _subscriberFactory = new Mock<IMessageSubscriberFactory>();
            private readonly Mock<IMessageSubscriber<PayeSchemeCreatedMessage>> _subscriber = new Mock<IMessageSubscriber<PayeSchemeCreatedMessage>>();
            private readonly Mock<IMessage<PayeSchemeCreatedMessage>> _logicalMessage = new Mock<IMessage<PayeSchemeCreatedMessage>>();
            private readonly PayeSchemeCreatedMessage _message = new PayeSchemeCreatedMessage();
            private readonly Mock<IActivityMapper> _activityMapper = new Mock<IActivityMapper>();
            private readonly Mock<IElasticClient> _client = new Mock<IElasticClient>();
            private Activity _activity;

            protected override void Given()
            {
                _logicalMessage.Setup(m => m.Content).Returns(_message);
                _subscriber.Setup(s => s.ReceiveAsAsync()).ReturnsAsync(_logicalMessage.Object).Callback(_cancellationTokenSource.Cancel);
                _subscriberFactory.Setup(s => s.GetSubscriber<PayeSchemeCreatedMessage>()).Returns(_subscriber.Object);
                _activityMapper.Setup(m => m.Map(It.IsAny<PayeSchemeCreatedMessage>(), ActivityType.PayeSchemeAdded, null, null));

                _client.Setup(c => c.IndexAsync(It.IsAny<Activity>(), It.IsAny<Func<IndexDescriptor<Activity>, IIndexRequest>>(), It.IsAny<CancellationToken>()))
                    .Callback<Activity, Func<IndexDescriptor<Activity>, IIndexRequest>, CancellationToken>((a, s, c) => _activity = a);

                _messageProcessor = new PayeSchemeCreatedMessageProcessor(_subscriberFactory.Object, Mock.Of<ILog>(), _activityMapper.Object, _client.Object);
            }

            protected override async Task When()
            {
                await _messageProcessor.RunAsync(_cancellationTokenSource.Token);
            }

            [Test]
            [Ignore("Broken until SFA.DAS.EmployerAccounts.Events is updated.")]
            public void Then_should_index_activity()
            {
                Assert.That(_activity, Is.Not.Null);
            }
        }
    }
}