using System;
using System.Threading.Tasks;
using SFA.DAS.Activities.Data;
using SFA.DAS.Activities.Models;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.Worker.MessageProcessors
{
    [TopicSubscription("Activity_PayeSchemeCreatedMessageProcessor")]
    public class PayeSchemeCreatedMessageProcessor : MessageProcessor<PayeSchemeCreatedMessage>
    {
        private readonly IActivitiesRepository _repository;

        public PayeSchemeCreatedMessageProcessor(IMessageSubscriberFactory subscriberFactory, ILog logger, IActivitiesRepository repository) : base(subscriberFactory, logger)
        {
            _repository = repository;
        }

        protected override async Task ProcessMessage(PayeSchemeCreatedMessage message)
        {
            await _repository.SaveActivity(new Activity
            {
                Type = ActivityTypeEnum.PayeSchemeAdded,
                /*AccountId = message.AccountId,
                At = message.CreatedAt,
                CreatorName = message.CreatorName,
                CreatorUserRef = message.CreatorUserRef
                PayeScheme = message.PayeScheme*/
                AccountId = 5,
                At = DateTime.UtcNow,
                CreatorName = "John Doe",
                CreatorUserRef = "04FCDEC7-5758-4BD2-A2D4-3E288E9EE047",
                PayeScheme = "333/AA00001"
            });
        }
    }
}