using System;
using System.Linq;
using System.Threading;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Messaging.Attributes;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.UnitTests
{
    public abstract class MessageProcessorTest<TMessageProcessor> where TMessageProcessor : IMessageProcessor2
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

            public void Verify(Func<MessageProcessorTestFixtures<TMessage>, IMessageProcessor2> createProcessor)
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var mappedActivity = new Activity();
                var fixtures = new MessageProcessorTestFixtures<TMessage>();

                fixtures.LogicalMessageMock.Setup(m => m.Content).Returns(_from);
                fixtures.SubscriberMock.Setup(s => s.ReceiveAsAsync()).ReturnsAsync(fixtures.LogicalMessageMock.Object)
                    .Callback(cancellationTokenSource.Cancel);
                fixtures.SubscriberFactoryMock.Setup(s => s.GetSubscriber<TMessage>()).Returns(fixtures.SubscriberMock.Object);

                var messageProcessor = createProcessor(fixtures);

                var topicSubscriptionAttribute =
                    typeof(TMessageProcessor).CustomAttributes.SingleOrDefault(a =>
                        a.AttributeType == typeof(TopicSubscriptionAttribute));
                var messageGroupAttribute =
                    typeof(TMessage).CustomAttributes.SingleOrDefault(a =>
                        a.AttributeType == typeof(MessageGroupAttribute));

                messageProcessor.RunAsync(cancellationTokenSource.Token).Wait();

                Assert.That(topicSubscriptionAttribute, Is.Not.Null);
                Assert.That(messageGroupAttribute, Is.Not.Null);
            }
        }
    }

    public class MessageProcessorTestFixtures<TMessage> where TMessage : class, new()
    {
        public Mock<ILog> LogMock = new Mock<ILog>();
        public ILog Log => LogMock.Object;

        public Mock<IMessageSubscriberFactory>  SubscriberFactoryMock = new Mock<IMessageSubscriberFactory>();
        public IMessageSubscriberFactory SubscriberFactory => SubscriberFactoryMock.Object;

        public Mock<IMessageSubscriber<TMessage>> SubscriberMock = new Mock<IMessageSubscriber<TMessage>>();
        public IMessageSubscriber<TMessage> Subscriber => SubscriberMock.Object;

        public Mock<IMessage<TMessage>> LogicalMessageMock = new Mock<IMessage<TMessage>>();
        public IMessage<TMessage> LogicalMessage { get; set; }

        public Mock<IActivitySaver> ActivitySaverMock = new Mock<IActivitySaver>();
        public IActivitySaver ActivitySaver => ActivitySaverMock.Object;

        public Mock<IMessageContextProvider> MessageContextProviderMock = new Mock<IMessageContextProvider>();
        public IMessageContextProvider MessageContextProvider => MessageContextProviderMock.Object;

    }
}