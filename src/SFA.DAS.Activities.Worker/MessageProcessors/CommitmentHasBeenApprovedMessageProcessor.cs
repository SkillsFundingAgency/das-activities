using System.Threading.Tasks;
using MediatR;
using NuGet;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;
using NuGetProject;
using SFA.DAS.Activities.Application;
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

        //protected override async Task ProcessMessage(CommitmentHasBeenApproved message)
        //{
        //    await _mediator.SendAsync(new SaveActivityCommand(
        //        new Activity()
        //            .WithActivityType(Activity.ActivityType.CommitmentHasBeenApproved)
        //            .WithDescription("A commitment has been approved")
        //            .WithOwnerId(message.OwnerId)
        //            .WithPostedDateTime(message.PostedDatedTime)
        //            .WithUrl(message.Url)));
        //}

        protected override async Task ProcessMessage(CommitmentHasBeenApproved message)
        {
            await _mediator.SendAsync(new SaveActivityCommand(
                new FluentActivity()
                .OwnerId(message.OwnerId)
                .ActivityType(Activity.ActivityType.CommitmentHasBeenApproved)
                .DescriptionSingular("commitment has been approved")
                .DescriptionPlural("commitments have been approved")
                .PostedDateTime(message.PostedDatedTime)
                .Url(message.Url)
                .Object()
                ));
        }
    }
}