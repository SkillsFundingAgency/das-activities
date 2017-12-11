﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuGet;
using SFA.DAS.Activities.Application.Repositories;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.Infrastructure.MessageProcessors
{
    [TopicSubscription("Activity_PayeSchemeAddedMessageProcessor")]
    public class PayeSchemeAddedMessageProcessor : MessageProcessor<PayeSchemeAddedMessage>
    {
        private readonly IActivitiesRepository _repository;

        public PayeSchemeAddedMessageProcessor(IMessageSubscriberFactory subscriberFactory, ILog logger, IActivitiesRepository repository) : base(subscriberFactory, logger)
        {
            _repository = repository;
        }

        protected override async Task ProcessMessage(PayeSchemeAddedMessage message)
        {
            var meta = new Dictionary<string, string> {{"PayeScheme", message.PayeScheme}};

            await _repository.SaveActivity(
                new Activity
                {
                    AccountId = message.AccountId,
                    Type = ActivityTypeEnum.PayeSchemeAdded,
                    At = message.CreatedAt,
                    Data = meta,
                    Creator = message.CreatorName
                }
            );
        }
    }
}