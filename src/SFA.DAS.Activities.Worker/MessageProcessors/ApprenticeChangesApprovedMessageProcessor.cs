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
    [TopicSubscription("Activity_ApprenticeChangesApprovedMessageProcessor")]
    public class ApprenticeChangesApprovedMessageProcessor : MessageProcessor<ApprenticeChangesApproved>
    {
        private readonly IMediator _mediator;

        public ApprenticeChangesApprovedMessageProcessor(IMessageSubscriberFactory subscriberFactory, ILog logger, IMediator mediator) : base(subscriberFactory, logger)
        {
            _mediator = mediator;
        }

        protected override async Task ProcessMessage(ApprenticeChangesApproved message)
        {
            await _mediator.SendAsync(new SaveActivityCommand(
                new FluentActivity()
                .OwnerId(message.OwnerId)
                .ActivityType(Activity.ActivityType.ApprenticeChangesApproved)
                .DescriptionSingular("apprentice change approved")
                .DescriptionPlural("apprentice changes approved")
                .PostedDateTime(message.PostedDatedTime)
                .Url(message.Url)
                .AddAssociatedThing(message.ApprenticeName)
                .Object()
                ));
        }
    }
}