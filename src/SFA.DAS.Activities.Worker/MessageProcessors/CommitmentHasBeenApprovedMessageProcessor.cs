using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;
using NuGetProject;
using NuGet;
using SFA.DAS.Activities.Application.Commands.SaveActivity;

namespace SFA.DAS.Activities.Worker.MessageProcessors
{
    public class CommitmentHasBeenApprovedMessageProcessor : MessageProcessor<CommitmentHasBeenApproved>
    {
        private readonly IMediator _mediator;

        public CommitmentHasBeenApprovedMessageProcessor(IPollingMessageReceiver pollingMessageReceiver, ILog logger, IMediator mediator) : base(pollingMessageReceiver, logger)
        {
            _mediator = mediator;
        }

        protected override async Task ProcessMessage(CommitmentHasBeenApproved message)
        {
            await _mediator.SendAsync(new SaveActivityCommand
                {
                   Payload = new FluentActivity()
                   .ActivityType(ActivityType.CommitmentHasBeenApproved.ToString())
                   .Description("A commitment has been approved")
                   .OwnerId(message.OwnerId)
                   .PostedDateTime(message.PostedDatedTime)
                   .Url(message.Url).Object()
                }
            );
        }
    }
}