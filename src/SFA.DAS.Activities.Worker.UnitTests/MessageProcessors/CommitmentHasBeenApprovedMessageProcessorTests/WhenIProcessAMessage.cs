using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NuGet;
using NUnit.Framework;
using SFA.DAS.Activities.Application.Commands.SaveActivity;
using SFA.DAS.Activities.Worker.MessageProcessors;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Tasks.Worker.UnitTests.MessageProcessors.CommitmentHasBeenApprovedMessageProcessorTests
{
    public class WhenIProcessAMessage
    {
        private const string OwnerId = "123";
        private DateTime _postedDateTime;

        private CohortApprovedMessageProcessor _processor;
        private Mock<IMessageSubscriberFactory> _subscriptionFactory;
        private Mock<IMessageSubscriber<CohortApproved>> _subscriber;
        private CancellationTokenSource _tokenSource;
        private Mock<IMediator> _mediator;
        private Mock<IMessage<CohortApproved>> _mockMessage;
        private CohortApproved _messageContent;

        [SetUp]
        public void Arrange()
        {
            _postedDateTime = DateTime.Parse("2015/10/25");

            _mockMessage = new Mock<IMessage<CohortApproved>>();

            _messageContent = new CohortApproved()
            {
                OwnerId = OwnerId,
                PostedDatedTime = _postedDateTime
            };

            _mockMessage.Setup(x => x.Content).Returns(_messageContent);

            _subscriptionFactory = new Mock<IMessageSubscriberFactory>();
            _subscriber = new Mock<IMessageSubscriber<CohortApproved>>();

            _mediator = new Mock<IMediator>();
            _tokenSource = new CancellationTokenSource();


            _processor = new CohortApprovedMessageProcessor(_subscriptionFactory.Object, Mock.Of<ILog>(), _mediator.Object);

            _subscriptionFactory.Setup(x => x.GetSubscriber<CohortApproved>()).Returns(_subscriber.Object);

            _subscriber.Setup(x => x.ReceiveAsAsync())
                .ReturnsAsync(() => _mockMessage.Object)
                .Callback(() => { _tokenSource.Cancel(); });
        }

        [Test]
        public async Task ThenTheMessageShouldBeHandledByAHandler()
        { 
            await _processor.RunAsync(_tokenSource.Token);

            _mediator.Verify(x => x.SendAsync(It.Is<SaveActivityCommand>(cmd => cmd.Activity.OwnerId.Equals(_messageContent.OwnerId.ToString()) &&
                                                                            cmd.Activity.Type == Activity.ActivityType.CohortApproved &&
                                                                            cmd.Activity.PostedDateTime == _postedDateTime)), Times.Once);
        }
    }
}
