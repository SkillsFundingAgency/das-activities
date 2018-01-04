﻿using System.Threading.Tasks;
using Nest;
using SFA.DAS.Activities.Worker.ObjectMappers;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.Worker.MessageProcessors
{
    [TopicSubscription("Activity_UserJoinedMessageProcessor")]
    public class UserJoinedMessageProcessor : MessageProcessor<UserJoinedMessage>
    {
        private readonly IActivityMapper _activityMapper;
        private readonly IElasticClient _client;

        public UserJoinedMessageProcessor(
            IMessageSubscriberFactory subscriberFactory, 
            ILog log, 
            IActivityMapper activityMapper, 
            IElasticClient client) 
            : base(subscriberFactory, log)
        {
            _activityMapper = activityMapper;
            _client = client;
        }

        protected override async Task ProcessMessage(UserJoinedMessage message)
        {
            var activity = _activityMapper.Map(message, ActivityType.UserJoined);
            await _client.IndexAsync(activity);
        }
    }
}
