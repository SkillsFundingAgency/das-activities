using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Tasks.Application.Commands.SaveTask;
using SFA.DAS.Tasks.API.Types.Enums;
using SFA.DAS.Tasks.Worker.MessageProcessors;

namespace SFA.DAS.Tasks.Worker.UnitTests.MessageProcessors.CreatedEmployerAgreementMessageProcessorTests
{
    public class WhenIProcessAMessage
    {
        private CreatedEmployerAgreementMessageProcessor _processor;
        private AgreementCreatedMessage _message;
        private Mock<IPollingMessageReceiver> _messageReciever;
        private CancellationTokenSource _tokenSource;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            _messageReciever = new Mock<IPollingMessageReceiver>();
            _mediator = new Mock<IMediator>();
            _tokenSource = new CancellationTokenSource();
            _message = new AgreementCreatedMessage
            {
                AccountId = 123,
                LegalEntityId = 456
            };

            _processor = new CreatedEmployerAgreementMessageProcessor(_messageReciever.Object, Mock.Of<ILog>(), _mediator.Object);

            _messageReciever.Setup(x => x.ReceiveAsAsync<AgreementCreatedMessage>())
                            .ReturnsAsync(() => new FileSystemMessage<AgreementCreatedMessage>(null, null, _message))
                            .Callback(() => { _tokenSource.Cancel(); });
        }

        [Test]
        public async Task ThenTheMessageShouldBeHandledByAHandler()
        {
            //Act
            await _processor.RunAsync(_tokenSource.Token);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<SaveTaskCommand>(cmd => cmd.OwnerId.Equals(_message.AccountId.ToString()) &&
                                                                            cmd.Type.Equals(TaskType.AgreementToSign) &&
                                                                            cmd.TaskCompleted.Equals(false))), Times.Once);
        }
    }
}
