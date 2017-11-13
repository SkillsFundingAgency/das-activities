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
    [TopicSubscription("Activity_PayeSchemeCreatedMessageProcessor")]
    public class PayeSchemeCreatedMessageProcessor : MessageProcessor<PayeSchemeCreatedMessage>
    {
        private readonly IMediator _mediator;

        public PayeSchemeCreatedMessageProcessor(IMessageSubscriberFactory subscriberFactory, ILog logger, IMediator mediator) : base(subscriberFactory, logger)
        {
            _mediator = mediator;
        }

        protected override async Task ProcessMessage(PayeSchemeCreatedMessage message)
        {
            await _mediator.SendAsync(new SaveActivityCommand(
                new FluentActivity()
                .AccountId(message.AccountId)
                .ActivityType(Activity.ActivityTypeStrings.AccountCreated)
                .DescriptionOne($"PAYE scheme {message.PayeSchemeRef} added")
                .DescriptionTwo($"At {message.PostedDatedTime.Format()} by {message.SignedByName}")
                .DescriptionSingular("PAYE scheme added")
                .DescriptionPlural("PAYE schemes added")
                .PostedDateTime(message.PostedDatedTime)
                .Url("todo")
                .Object()
                ));
        }
    }
}