using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.Exceptions;
using SFA.DAS.Activities.IntegrityChecker.Dto;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.Repositories
{
    public class CosmosActivityDocumentRepository : ICosmosActivityDocumentRepository
    {
        private readonly ICosmosClient _cosmosClient;
        private readonly ICosmosConfiguration _config;

        public CosmosActivityDocumentRepository(ICosmosClient cosmosClient, ICosmosConfiguration config)
        {
            _cosmosClient = cosmosClient;
            _config = config;
        }

	    public Task UpsertActivityAsync(Activity activity)
	    {
		    return _cosmosClient.UpsertDocumentAsync(_config.CosmosCollectionName, new CosmosActivity(activity));
	    }

	    public async Task<Activity[]> GetActivitiesAsync(IPagingData pagingData)
        {
            if (!(pagingData is CosmosPagingData cosmosPagingData))
            {
                throw new InvalidQueryPagingData(
                    $"paging data supplied to {nameof(CosmosActivityDocumentRepository)} is not a {nameof(CosmosPagingData)}");
            }

            var cosmosResults = await _cosmosClient.GetPageAsync<CosmosActivity, Guid>(_config.CosmosCollectionName, cosmosPagingData.ContinuationToken, documentType => documentType.MessageId, pagingData.RequiredPageSize);

            cosmosPagingData.ContinuationToken = cosmosResults.ContinuationToken;

	        var activities = cosmosResults.Items.Select(ca => ca.Activity).ToArray();
            cosmosPagingData.CurrentPageSize = activities.Length;

	        activities.AssertInIdOrder();

			return activities;
        }

        public async Task<Activity> GetActivityAsync(Guid messageId)
        {
            var cosmosActivity = await _cosmosClient.GetDocumentAsync<CosmosActivity>(_config.CosmosCollectionName, activity => activity.Id == messageId);
            return cosmosActivity.Activity;
        }

        public Task DeleteActivityAsync(Guid messageId)
        {
            return _cosmosClient.DeleteDocumentsAsync<CosmosActivity>(_config.CosmosCollectionName, activity => activity.Id == messageId);
        }

        public Task DeleteAllActivitiesFromRepoAsync()
        {
            return _cosmosClient.RecreateCollection(_config.CosmosCollectionName);
        }
    }
}