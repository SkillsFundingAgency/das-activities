using System.Threading.Tasks;
using MediatR;
using NuGet;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Activities.Application.Commands.SaveActivity;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Activities.WorkerRole.MessageProcessors
{
    [TopicSubscription("Activity_LegalEntityRemovedMessageProcessor")]
    public class LegalEntityRemovedMessageProcessor : MessageProcessor<LegalEntityRemovedMessage>
    {
        private readonly IMediator _mediator;

        public LegalEntityRemovedMessageProcessor(IMessageSubscriberFactory subscriberFactory, ILog logger, IMediator mediator) : base(subscriberFactory, logger)
        {
            _mediator = mediator;
        }

        protected override async Task ProcessMessage(LegalEntityRemovedMessage message)
        {
            await _mediator.SendAsync(new SaveActivityCommand(
                new FluentActivity()
                .AccountId(message.AccountId)
                .ActivityType(Activity.ActivityTypeStrings.LegalEntityRemoved)
                .DescriptionOne("Account created")
                .DescriptionTwo($"At {message.PostedDatedTime.Format()} by {message.RemovedByName}")
                .DescriptionSingular("account created")
                .DescriptionPlural("accounts created")
                .PostedDateTime(message.PostedDatedTime)
                .Url("todo")
                .Object()
                ));
        }
    }
}