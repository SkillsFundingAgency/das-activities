using System.Threading.Tasks;
using MediatR;
using NuGet;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Activities.Application.Commands.SaveActivity;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Activities.Worker.MessageProcessors
{
    [TopicSubscription("Activity_AgreementSignedMessageProcessor")]
    public class AgreementSignedMessageProcessor : MessageProcessor<AgreementSignedMessage>
    {
        private readonly IMediator _mediator;

        public AgreementSignedMessageProcessor(IMessageSubscriberFactory subscriberFactory, ILog logger, IMediator mediator) : base(subscriberFactory, logger)
        {
            _mediator = mediator;
        }

        protected override async Task ProcessMessage(AgreementSignedMessage message)
        {
            await _mediator.SendAsync(new SaveActivityCommand(
                new FluentActivity()
                .AccountId(message.AccountId)
                .ActivityType(Activity.ActivityTypeStrings.AgreementSigned)
                .DescriptionOne($"agreement signed with {message.ProviderName}")
                .DescriptionTwo($"At {message.PostedDatedTime.Format()} by {message.SignedByName}")
                .DescriptionSingular("agreement signed")
                .DescriptionPlural("agreements signed")
                .PostedDateTime(message.PostedDatedTime)
                .Url("todo")
                .Object()
                ));
        }
    }
}