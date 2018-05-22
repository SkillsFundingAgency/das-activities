using System;
using System.Text;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.Activities.Configuration;
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
        private readonly ICosmosClient _cosmosClient;
        private readonly IElasticClient _elasticClient;
        private readonly ILog _logger;
        private readonly IMessageContextProvider _messageContextProvider;
        private readonly IMessageServiceBusConfiguration _config;

        public ActivitySaver(
            IActivityMapper activityMapper,
            ICosmosClient cosmosClient,
            IElasticClient elasticClient,
            ILog logger,
            IMessageContextProvider messageContextProvider,
            IMessageServiceBusConfiguration messageServiceBusConfiguration
        )
        {
            StringBuilder errors = null;
            AssertConstructorArgument(nameof(activityMapper), activityMapper, ref errors);
            AssertConstructorArgument(nameof(cosmosClient), cosmosClient, ref errors);
            AssertConstructorArgument(nameof(elasticClient), elasticClient, ref errors);
            AssertConstructorArgument(nameof(messageContextProvider), messageContextProvider, ref errors);
            AssertConstructorArgument(nameof(messageServiceBusConfiguration), messageServiceBusConfiguration, ref errors);
            AssertConstructorArgument(nameof(messageServiceBusConfiguration.CosmosDatabase), messageServiceBusConfiguration?.CosmosDatabase, ref errors);
            AssertConstructorArgument(nameof(messageServiceBusConfiguration.CosmosCollectionName), messageServiceBusConfiguration?.CosmosCollectionName, ref errors);
            AssertConstructorArgument(nameof(messageServiceBusConfiguration.CosmosEndpointUrl), messageServiceBusConfiguration?.CosmosEndpointUrl, ref errors);
            AssertConstructorArgument(nameof(messageServiceBusConfiguration.CosmosPrimaryKey), messageServiceBusConfiguration?.CosmosPrimaryKey, ref errors);

            if (errors != null)
            {
                throw new ArgumentException(errors.ToString());
            }

            _activityMapper = activityMapper;
            _cosmosClient = cosmosClient;
            _elasticClient = elasticClient;
            _messageContextProvider = messageContextProvider;
            _config = messageServiceBusConfiguration;
        }

        public async Task SaveActivity<TMessage>(TMessage message, ActivityType activityType) where TMessage : class
        {
            var messageContext = _messageContextProvider.GetContextForMessageBody(message);

            var activity = _activityMapper.Map(message, activityType, messageId: messageContext.MessageId);

            await RunWithTry(() => _cosmosClient.UpsertDocumentAsync(_config.CosmosCollectionName, activity), "Saving activity to CosmosDB");
            await RunWithTry(() => _elasticClient.IndexAsync(activity), "Saving activity to Elastic");
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

        private void AssertConstructorArgument(string fieldName, string fieldValue, ref StringBuilder errors)
        {
            if (string.IsNullOrWhiteSpace(fieldValue))
            {
                errors = errors ?? new StringBuilder();
                errors.AppendLine($"\"{fieldName}\" has no value (it is either null or white space)");
            }
        }

        private void AssertConstructorArgument(string argumentName, object argumentValue, ref StringBuilder errors)
        {
            if (argumentValue == null)
            {
                errors = errors ?? new StringBuilder();
                errors.AppendLine($"\"{argumentName}\" is null");
            }
        }
    }
}