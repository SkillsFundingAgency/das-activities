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
    [TopicSubscription("Activity_PayeSchemeDeletedMessageProcessor")]
    public class PayeSchemeDeletedMessageProcessor : MessageProcessor<PayeSchemeDeletedMessage>
    {
        private readonly IMediator _mediator;

        public PayeSchemeDeletedMessageProcessor(IMessageSubscriberFactory subscriberFactory, ILog logger, IMediator mediator) : base(subscriberFactory, logger)
        {
            _mediator = mediator;
        }

        protected override async Task ProcessMessage(PayeSchemeDeletedMessage message)
        {
            await _mediator.SendAsync(new SaveActivityCommand(
                new FluentActivity()
                .AccountId(message.AccountId)
                .ActivityType(Activity.ActivityTypeStrings.AccountCreated)
                .DescriptionOne($"PAYE scheme {message.EmpRef} removed")
                .DescriptionTwo($"At {message.PostedDatedTime.Format()} by {message.DeletedByName}")
                .DescriptionSingular("PAYE scheme removed")
                .DescriptionPlural("PAYE schemes removed")
                .PostedDateTime(message.PostedDatedTime)
                .Url("todo")
                .Object()
                ));
        }
    }
}