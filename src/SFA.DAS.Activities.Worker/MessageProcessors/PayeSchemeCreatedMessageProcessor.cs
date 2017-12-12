using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Activities.Client;
using SFA.DAS.Activities.Worker.Services;
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
        private readonly IActivitiesService _activitiesService;

        public PayeSchemeCreatedMessageProcessor(IMessageSubscriberFactory subscriberFactory, ILog logger, IActivitiesService activitiesService)
            : base(subscriberFactory, logger)
        {
            _activitiesService = activitiesService;
        }

        protected override async Task ProcessMessage(PayeSchemeCreatedMessage message)
        {
            await _activitiesService.AddActivity(new Activity
            {
                Type = ActivityType.PayeSchemeAdded,
                /*AccountId = message.AccountId,
                At = message.CreatedAt,
                Data = new Dictionary<string, object>
                {
                    ["CreatorUserRef"] = message.CreatorUserRef,
                    ["CreatorName"] = message.CreatorName,
                    ["PayeScheme"] = message.PayeScheme
                },
                Keywords = new List<string>
                {
                    message.CreatorUserRef,
                    message.CreatorName,
                    message.PayeScheme
                }*/
                AccountId = 5,
                At = DateTime.UtcNow,
                Data = new Dictionary<string, string>
                {
                    ["CreatorUserRef"] = "04FCDEC7-5758-4BD2-A2D4-3E288E9EE047",
                    ["CreatorName"] = "John Doe",
                    ["PayeScheme"] = "333/AA00001"
                },
                Keywords = new List<string>
                {
                    "04FCDEC7-5758-4BD2-A2D4-3E288E9EE047",
                    "John Doe",
                    "333/AA00001"
                }
            });
        }
    }
}