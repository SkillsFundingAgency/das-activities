using System;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.Activities.Exceptions;
using SFA.DAS.Activities.Extensions;
using SFA.DAS.Activities.IntegrityChecker.Dto;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.Repositories
{
    public class ElasticActivityDocumentRepository : IElasticActivityDocumentRepository
    {
        private readonly IElasticClient _client;

        public ElasticActivityDocumentRepository(IElasticClient client)
        {
            _client = client;
        }

	    public Task UpsertActivityAsync(Activity activity)
	    {
		    return _client.IndexAsync(activity);
	    }

	    public async Task<Activity[]> GetActivitiesAsync(IPagingData pagingData)
        {
            if (!(pagingData is ElasticPagingData elasticPagingData))
            {
                throw new InvalidQueryPagingData(
                    $"paging data supplied to {nameof(ElasticActivityDocumentRepository)} is not a {nameof(ElasticPagingData)}");
            }

            // See https://www.elastic.co/guide/en/elasticsearch/guide/current/pagination.html
            // for a description of deep paging issues

	        var dbQuery = await _client.SearchAsync<Activity>(s => s
		        .From(elasticPagingData.FromIndex)
		        .Size(pagingData.RequiredPageSize)
		        .Sort(srt => srt.Ascending(a => a. Id))
		        .Query(q => q.MatchAll()));

            var activities = dbQuery.Documents.ToArray();

            elasticPagingData.FromIndex += activities.Length;
            elasticPagingData.CurrentPageSize = activities.Length;
			activities.AssertInIdOrder();

            return activities;
        }

        public async Task<Activity> GetActivityAsync(Guid messageId)
        {
            var result = await _client.SearchAsync<Activity>(s =>
                s.Query(q => q.Bool(b => b.Filter(bf => bf.Ids(iqd => new IdsQuery {Values = new []{ new Id(messageId.ToString())}})))));

            var activity = result.Documents.SingleOrDefault();
            return activity;
        }

        public Task DeleteActivityAsync(Guid messageId)
        {
            return _client.DeleteAsync<Activity>(messageId);
        }

        public Task DeleteAllActivitiesFromRepoAsync()
        {
            return _client.DeleteByQueryAsync<Activity>(del => del
                .Query(q => q.QueryString(qs => qs.Query("*"))));
        }
    }
}