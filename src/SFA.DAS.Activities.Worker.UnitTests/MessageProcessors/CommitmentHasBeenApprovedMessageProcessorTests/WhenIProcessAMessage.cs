﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NuGet;
using NuGetProject;
using NUnit.Framework;
using SFA.DAS.Activities.Application.Commands.CommitmentHasBeenApproved;
using SFA.DAS.Activities.Worker;
using SFA.DAS.Activities.Worker.MessageProcessors;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Tasks.Worker.UnitTests.MessageProcessors.CommitmentHasBeenApprovedMessageProcessorTests
{
    public class WhenIProcessAMessage
    {
        private const string OwnerId = "123";
        private DateTime _postedDateTime;
        private string _postedDateTimeString;

        private CommitmentHasBeenApprovedMessageProcessor _processor;
        private CommitmentHasBeenApproved _message;
        private Mock<IPollingMessageReceiver> _messageReciever;
        private CancellationTokenSource _tokenSource;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            _postedDateTime = DateTime.Parse("2015/10/25");
            _postedDateTimeString = _postedDateTime.ToString("O");

            _messageReciever = new Mock<IPollingMessageReceiver>();
            _mediator = new Mock<IMediator>();
            _tokenSource = new CancellationTokenSource();
            _message = new CommitmentHasBeenApproved
            {
                OwnerId = OwnerId,
                PostedDatedTime = _postedDateTimeString
            };

            _processor = new CommitmentHasBeenApprovedMessageProcessor(_messageReciever.Object, Mock.Of<ILog>(), _mediator.Object);

            _messageReciever.Setup(x => x.ReceiveAsAsync<CommitmentHasBeenApproved>())
                            .ReturnsAsync(() => new FileSystemMessage<CommitmentHasBeenApproved>(null, null, _message))
                            .Callback(() => { _tokenSource.Cancel(); });
        }

        [Test]
        public async Task ThenTheMessageShouldBeHandledByAHandler()
        { 
            await _processor.RunAsync(_tokenSource.Token);


            _mediator.Verify(x => x.SendAsync(It.Is<CommitmentHasBeenApprovedCommand>(cmd => cmd.PayLoad.OwnerId.Equals(_message.OwnerId.ToString()) &&
                                                                            cmd.PayLoad.ActivityType == ActivityType.CommitmentHasBeenApproved.ToString() &&
                                                                            cmd.PayLoad.PostedDateTime == _postedDateTime)), Times.Once);
        }
    }
}
