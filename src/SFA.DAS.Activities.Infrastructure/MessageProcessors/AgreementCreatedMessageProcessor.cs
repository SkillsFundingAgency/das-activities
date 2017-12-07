﻿//using System.Threading.Tasks;
//using MediatR;
//using NuGet;
//using SFA.DAS.Messaging;
//using SFA.DAS.NLog.Logger;
//using SFA.DAS.Activities.Application.Commands.SaveActivity;
//using SFA.DAS.EmployerAccounts.Events.Messages;
//using SFA.DAS.Messaging.AzureServiceBus.Attributes;
//using SFA.DAS.Messaging.Interfaces;

//namespace SFA.DAS.Activities.Worker.MessageProcessors
//{
//    [TopicSubscription("Activity_AgreementCreatedMessageProcessor")]
//    public class AgreementCreatedMessageProcessor : MessageProcessor<AgreementCreatedMessage>
//    {
//        private readonly IMediator _mediator;

//        public AgreementCreatedMessageProcessor(IMessageSubscriberFactory subscriberFactory, ILog logger, IMediator mediator) : base(subscriberFactory, logger)
//        {
//            _mediator = mediator;
//        }

//        protected override async Task ProcessMessage(AgreementCreatedMessage message)
//        {
//            await _mediator.SendAsync(new SaveActivityCommand(
//                new FluentActivity()
//                .AccountId(message.AccountId)
//                .ActivityType(Activity.ActivityTypeStrings.AgreementSigned)
//                .DescriptionOne($"agreement created with {message.OrganisationName}")
//                .DescriptionTwo($"At {message.PostedDatedTime.Format()} by {message.CreatedByName}")
//                .DescriptionSingular("agreement created")
//                .DescriptionPlural("agreements created")
//                .PostedDateTime(message.PostedDatedTime)
//                .Url("todo")
//                .Object()
//                ));
//        }
//    }
//}