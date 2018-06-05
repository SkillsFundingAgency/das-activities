using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Newtonsoft.Json;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.Exceptions;
using SFA.DAS.Activities.IntegrityChecker.Dto;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.Repositories
{
    /// <remarks>
    ///     When serialising objects DocumentDb will respect the json serialisation settings, which is what converts the property "Id" to the
    ///     "id" property required by documentdb.
    ///     However, LINQ driver does not uses the serialisation settings - instead it uses the type property directly, only looking for a
    ///     JSONProperty attribute on the property. Consequently a LINQ expression of activity.Id == 223 will actually look for a property
    ///     named "Id", not the one stored "id".
    ///     
    ///     See https://github.com/Azure/azure-documentdb-dotnet/issues/317
    /// 
    ///     This class will map between Activity and CosmosProperty, the latter having the appropriate JSONProperty attributes to allow the 
    ///     LINQ to work.
    /// </remarks>>
    internal class CosmosActivity
    {
 
        public CosmosActivity(Activity activity)
        {
            Activity = activity;
        }

        public CosmosActivity()
        {
            Activity = new Activity();
        }

        public Activity Activity { get; }

        [JsonProperty("id")]
        public string Id
        {
            get => Activity.Id;
            set => Activity.Id = value;
        }

		/// <summary>
		///		We cannot order by Id in Cosmos, so have to store separately so that a range index is created over it.
		/// </summary>
	    [JsonProperty("messageId")]
	    public string MessageId => Activity.Id;

		[JsonProperty("accountId")]
        public long AccountId
        {
            get => Activity.AccountId;
            set => Activity.AccountId = value;
        }

        [JsonProperty("at")]
        public DateTime At
        {
            get => Activity.At;
            set => Activity.At = value;
        }

        [JsonProperty("created")]
        public DateTime Created
        {
            get => Activity.Created;
            set => Activity.Created = value;
        }

        [JsonProperty("data")]
        public Dictionary<string, string> Data
        {
            get => Activity.Data;
            set => Activity.Data = value;
        }

        [JsonProperty("description")]
        public string Description
        {
            get => Activity.Description;
            set => Activity.Description = value;
        }

        [JsonProperty("type")]
        public ActivityType Type
        {
            get => Activity.Type;
            set => Activity.Type = value;
        }
    }

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

            var cosmosResults = await _cosmosClient.GetPageAsync<CosmosActivity, string>(_config.CosmosCollectionName, cosmosPagingData.ContinuationToken, documentType => documentType.MessageId, pagingData.RequiredPageSize);

            cosmosPagingData.ContinuationToken = cosmosResults.ContinuationToken;

	        var activities = cosmosResults.Items.Select(ca => ca.Activity).ToArray();

	        activities.AssertInIdOrder();

			return activities;
        }

        public async Task<Activity> GetActivityAsync(string messageId)
        {
            var cosmosActivity = await _cosmosClient.GetDocumentAsync<CosmosActivity>(_config.CosmosCollectionName, activity => activity.Id == messageId);
            return cosmosActivity.Activity;
        }

        public Task DeleteActivityAsync(string messageId)
        {
            return _cosmosClient.DeleteDocumentsAsync<CosmosActivity>(_config.CosmosCollectionName, activity => activity.Id == messageId);
        }

        public Task DeleteAllActivitiesFromRepoAsync()
        {
            return _cosmosClient.RecreateCollection(_config.CosmosCollectionName);
        }
    }
}