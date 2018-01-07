using System;
using System.Linq;
using System.Threading;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Activities.Worker.ObjectMappers;
using SFA.DAS.Messaging.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.UnitTests.Worker.MessageProcessors
{
    public abstract class MessageProcessorTest<TMessageProcessor> where TMessageProcessor : IMessageProcessor
    {
        protected FromMessage<TMessage> From<TMessage>(TMessage from) where TMessage : class, new()
        {
            return new FromMessage<TMessage>(from);
        }

        protected class FromMessage<TMessage> where TMessage : class, new()
        {
            private readonly TMessage _from;

            public FromMessage(TMessage from)
            {
                _from = from;
            }

            public ToActivity<TMessage> To(ActivityType to)
            {
                return new ToActivity<TMessage>(_from, to);
            }
        }

        protected class ToActivity<TMessage> where TMessage : class, new()
        {
            private readonly TMessage _from;
            private readonly ActivityType _to;
            private Func<TMessage, long> _accountId;
            private Func<TMessage, DateTime> _createdAt;

            public ToActivity(TMessage from, ActivityType to)
            {
                _from = from;
                _to = to;
            }

            public ToActivity<TMessage> WithAccountId(Func<TMessage, long> accountId)
            {
                _accountId = accountId;
                return this;
            }

            public ToActivity<TMessage> WithCreatedAt(Func<TMessage, DateTime> createdAt)
            {
                _createdAt = createdAt;
                return this;
            }

            public void Verify()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var subscriberFactory = new Mock<IMessageSubscriberFactory>();
                var subscriber = new Mock<IMessageSubscriber<TMessage>>();
                var logicalMessage = new Mock<IMessage<TMessage>>();
                var activityMapper = new Mock<IActivityMapper>();
                var client = new Mock<IElasticClient>();
                var mappedActivity = new Activity();

                Activity indexedActivity = null;

                logicalMessage.Setup(m => m.Content).Returns(_from);
                subscriber.Setup(s => s.ReceiveAsAsync()).ReturnsAsync(logicalMessage.Object).Callback(cancellationTokenSource.Cancel);
                subscriberFactory.Setup(s => s.GetSubscriber<TMessage>()).Returns(subscriber.Object);

                activityMapper.Setup(m => m.Map(_from, _to, It.IsAny<Func<TMessage, long>>(), It.IsAny<Func<TMessage, DateTime>>()))
                    .Callback<TMessage, ActivityType, Func<TMessage, long>, Func<TMessage, DateTime>>((m, t, a, c) =>
                    {
                        if (a != null)
                        {
                            mappedActivity.AccountId = a(_from);
                        }

                        if (c != null)
                        {
                            mappedActivity.At = c(_from);
                        }
                    })
                    .Returns(mappedActivity);

                client.Setup(c => c.IndexAsync(mappedActivity, It.IsAny<Func<IndexDescriptor<Activity>, IIndexRequest>>(), It.IsAny<CancellationToken>()))
                    .Callback<Activity, Func<IndexDescriptor<Activity>, IIndexRequest>, CancellationToken>((a, s, c) => indexedActivity = a);

                var messageProcessor = (TMessageProcessor)Activator.CreateInstance(typeof(TMessageProcessor), subscriberFactory.Object, Mock.Of<ILog>(), activityMapper.Object, client.Object);
                var topicSubscriptionAttribute = typeof(TMessageProcessor).CustomAttributes.SingleOrDefault(a => a.AttributeType == typeof(TopicSubscriptionAttribute));
                var messageGroupAttribute = typeof(TMessage).CustomAttributes.SingleOrDefault(a => a.AttributeType == typeof(MessageGroupAttribute));

                messageProcessor.RunAsync(cancellationTokenSource.Token).Wait();

                Assert.That(topicSubscriptionAttribute, Is.Not.Null);
                Assert.That(messageGroupAttribute, Is.Not.Null);
                Assert.That(indexedActivity, Is.Not.Null);
                Assert.That(indexedActivity, Is.SameAs(mappedActivity));

                if (_accountId != null)
                {
                    Assert.That(indexedActivity.AccountId, Is.EqualTo(_accountId(_from)));
                }

                if (_createdAt != null)
                {
                    Assert.That(indexedActivity.At, Is.EqualTo(_createdAt(_from)));
                }
            }
        }
    }
}