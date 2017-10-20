using System.Threading.Tasks;
using MediatR;
using NuGet;
using NuGet.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Activities.Application.Commands.SaveActivity;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Activities.Worker.MessageProcessors
{
    [TopicSubscription("Activity_CommitmentHasBeenApprovedMessageProcessor")]
    public class CommitmentHasBeenApprovedMessageProcessor : MessageProcessor<CommitmentHasBeenApproved>
    {
        private readonly IMediator _mediator;

        public CommitmentHasBeenApprovedMessageProcessor(IMessageSubscriberFactory subscriberFactory, ILog logger, IMediator mediator) : base(subscriberFactory, logger)
        {
            _mediator = mediator;
        }

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