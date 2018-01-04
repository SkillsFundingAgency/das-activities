using System;
using System.Threading;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Activities.Worker.ObjectMappers;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.UnitTests.Worker.MessageProcessors
{
    public abstract class MessageProcessorTest<TMessageProcessor> where TMessageProcessor : IMessageProcessor
    {
        protected void Test<TMessage>(TMessage message, ActivityType type, Func<TMessage, long> accountId = null, Func<TMessage, DateTime> createdAt = null) where TMessage : class, new()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var subscriberFactory = new Mock<IMessageSubscriberFactory>();
            var subscriber = new Mock<IMessageSubscriber<TMessage>>();
            var logicalMessage = new Mock<IMessage<TMessage>>();
            var activityMapper = new Mock<IActivityMapper>();
            var client = new Mock<IElasticClient>();
            var mappedActivity = new Activity();
            
            Activity indexedActivity = null;

            logicalMessage.Setup(m => m.Content).Returns(message);
            subscriber.Setup(s => s.ReceiveAsAsync()).ReturnsAsync(logicalMessage.Object).Callback(cancellationTokenSource.Cancel);
            subscriberFactory.Setup(s => s.GetSubscriber<TMessage>()).Returns(subscriber.Object);
            
            activityMapper.Setup(m => m.Map(message, type, It.IsAny<Func<TMessage, long>>(), It.IsAny<Func<TMessage, DateTime>>()))
                .Callback<TMessage, ActivityType, Func<TMessage, long>, Func<TMessage, DateTime>>((m, t, a, c) =>
                {
                    if (a != null)
                    {
                        mappedActivity.AccountId = a(message);
                    }

                    if (c != null)
                    {
                        mappedActivity.At = c(message);
                    }
                })
                .Returns(mappedActivity);

            client.Setup(c => c.IndexAsync(mappedActivity, It.IsAny<Func<IndexDescriptor<Activity>, IIndexRequest>>(), It.IsAny<CancellationToken>()))
                .Callback<Activity, Func<IndexDescriptor<Activity>, IIndexRequest>, CancellationToken>((a, s, c) => indexedActivity = a);

            var messageProcessor = (TMessageProcessor)Activator.CreateInstance(typeof(TMessageProcessor), subscriberFactory.Object, Mock.Of<ILog>(), activityMapper.Object, client.Object);

            messageProcessor.RunAsync(cancellationTokenSource.Token).Wait();

            Assert.That(indexedActivity, Is.Not.Null);
            Assert.That(indexedActivity, Is.SameAs(mappedActivity));

            if (accountId != null)
            {
                Assert.That(indexedActivity.AccountId, Is.EqualTo(accountId(message)));
            }

            if (createdAt != null)
            {
                Assert.That(indexedActivity.At, Is.EqualTo(createdAt(message)));
            }
        }
    }
}