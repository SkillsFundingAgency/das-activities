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
    [TopicSubscription("Activity_CohortApprovedMessageProcessor")]
    public class CohortApprovedMessageProcessor : MessageProcessor<CohortApproved>
    {
        private readonly IMediator _mediator;

        public CohortApprovedMessageProcessor(IMessageSubscriberFactory subscriberFactory, ILog logger, IMediator mediator) : base(subscriberFactory, logger)
        {
            _mediator = mediator;
        }

        protected override async Task ProcessMessage(CohortApproved message)
        {
            await _mediator.SendAsync(new SaveActivityCommand(
                new FluentActivity()
                .OwnerId(message.OwnerId)
                .ActivityType(Activity.ActivityType.ApprenticeChangesApproved)
                .DescriptionSingular($"cohort approved with {message.ProviderName}")
                .DescriptionPlural("cohorts approved")
                .PostedDateTime(message.PostedDatedTime)
                .Url(message.Url)
                .AddAssociatedThing(message.ProviderName)
                .AddAssociatedThings(message.Apprentices)
                .Object()
                ));
        }
    }
}