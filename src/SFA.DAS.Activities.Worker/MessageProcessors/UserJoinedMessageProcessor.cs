//using System.Threading.Tasks;
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
//    [TopicSubscription("Activity_UserJoinedMessageProcessor")]
//    public class UserJoinedMessageProcessor : MessageProcessor<UserJoinedMessage>
//    {
//        private readonly IMediator _mediator;

//        public UserJoinedMessageProcessor(IMessageSubscriberFactory subscriberFactory, ILog logger, IMediator mediator) : base(subscriberFactory, logger)
//        {
//            _mediator = mediator;
//        }

//        protected override async Task ProcessMessage(UserJoinedMessage message)
//        {
//            await _mediator.SendAsync(new SaveActivityCommand(
//                new FluentActivity()
//                .AccountId(message.AccountId)
//                .ActivityType(Activity.ActivityTypeStrings.UserJoined)
//                .DescriptionOne($"{message.WhoJoinedName} joined")
//                .DescriptionTwo($"At {message.PostedDatedTime.Format()}")
//                .DescriptionSingular("user joined")
//                .DescriptionPlural("users joined")
//                .PostedDateTime(message.PostedDatedTime)
//                .Url("todo")
//                .Object()
//                ));
//        }
//    }
//}