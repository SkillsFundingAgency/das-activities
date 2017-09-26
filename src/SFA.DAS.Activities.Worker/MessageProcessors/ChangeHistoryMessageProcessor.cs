using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Activities.Application.Commands.SaveActivity;
using SFA.DAS.Activities.API.Types.Enums;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;
//using SFA.DAS.EmployerAccounts.Events.Messages;


namespace SFA.DAS.Activities.Worker.MessageProcessors
{
    public class CreatedEmployerAgreementMessageProcessor : MessageProcessor<ChangeHistoryMessage>
    {
        private readonly IMediator _mediator;

        public CreatedEmployerAgreementMessageProcessor(IPollingMessageReceiver pollingMessageReceiver, ILog logger, IMediator mediator) : base(pollingMessageReceiver, logger)
        {
            _mediator = mediator;
        }

        protected override async Task ProcessMessage(ChangeHistoryMessage message)
        {
            await _mediator.SendAsync(new SaveActivityCommand
            {
                OwnerId = message.AccountId.ToString(),
                Type = ActivityType.ChangeHistory,
                Description = message.Description
            });
        }
    }
}