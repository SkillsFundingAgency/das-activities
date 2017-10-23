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
    [TopicSubscription("Activity_ApprenticeChangesRequestedMessageProcessor")]
    public class ApprenticeChangesRequestedMessageProcessor : MessageProcessor<ApprenticeChangesRequested>
    {
        private readonly IMediator _mediator;

        public ApprenticeChangesRequestedMessageProcessor(IMessageSubscriberFactory subscriberFactory, ILog logger, IMediator mediator) : base(subscriberFactory, logger)
        {
            _mediator = mediator;
        }

        protected override async Task ProcessMessage(ApprenticeChangesRequested message)
        {
            await _mediator.SendAsync(new SaveActivityCommand(
                new FluentActivity()
                .OwnerId(message.OwnerId)
                .ActivityType(Activity.ActivityType.ApprenticeChangesRequested)
                .DescriptionSingular("apprentice change requested")
                .DescriptionPlural("apprentice changes requested")
                .PostedDateTime(message.PostedDatedTime)
                .Url(message.Url)
                .Object()
                ));
        }
    }
}