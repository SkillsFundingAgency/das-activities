using System;
using System.Text;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;
using SFA.DAS.Activities.Worker.MessageProcessors;
using SFA.DAS.Activities.Worker.ObjectMappers;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using StructureMap.TypeRules;

namespace SFA.DAS.Activities.Worker.ActivitySavers
{
    public class ActivitySaver : IActivitySaver
    {
        private readonly IActivityMapper _activityMapper;
        private readonly ICosmosActivityDocumentRepository _cosmosActivityRepository;
        private readonly IElasticActivityDocumentRepository _elasticActivityRepository;
        private readonly ILog _logger;
        private readonly IMessageContextProvider _messageContextProvider;

        public ActivitySaver(
            IActivityMapper activityMapper,
            ICosmosActivityDocumentRepository cosmosActivityRepository,
            IElasticActivityDocumentRepository elasticActivityRepository,
            ILog logger,
            IMessageContextProvider messageContextProvider
        )
        {
            _activityMapper = activityMapper;
            _cosmosActivityRepository = cosmosActivityRepository;
            _elasticActivityRepository = elasticActivityRepository;
            _logger = logger;
            _messageContextProvider = messageContextProvider;
        }

        public async Task<Activity> SaveActivity<TMessage>(TMessage message, ActivityType activityType) where TMessage : class
        {
            var messageContext = _messageContextProvider.GetContextForMessageBody(message);

	        if (!Guid.TryParse(messageContext.MessageId, out Guid messageId))
	        {
		        throw new Exception($"The attempt to save activity from message type {typeof(TMessage).FullName} failed because the message id supplied by {nameof(IMessageContextProvider)}.{nameof(IMessageContextProvider.GetContextForMessageBody)} is not a GUID (the value provided was {messageContext.MessageId}");
	        }

            var activity = _activityMapper.Map(message, activityType, messageId: messageId);

            await RunWithTry(() => _cosmosActivityRepository.UpsertActivityAsync(activity), "Saving activity to CosmosDB");
            await RunWithTry(() => _elasticActivityRepository.UpsertActivityAsync(activity), "Saving activity to Elastic");

            return activity;
        }

        private async Task RunWithTry(Func<Task> run, string description)
        {
            try
            {
                await run();
            }
            catch (Exception e)
            {
                _logger.Error(e, $"operation failed: {description}");
                throw;
            }
        }
    }
}