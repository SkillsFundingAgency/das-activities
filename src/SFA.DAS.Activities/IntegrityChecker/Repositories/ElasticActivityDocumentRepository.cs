using System.Linq;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.Activities.Exceptions;
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
                .MatchAll()
                .Sort(srt => srt.Ascending(a => a.Id))
                .From(elasticPagingData.FromIndex)
                .Take(pagingData.RequiredPageSize));

            var result = dbQuery.Documents.ToArray();

            elasticPagingData.FromIndex += result.Length;
            elasticPagingData.MoreDataAvailable = result.Length == pagingData.RequiredPageSize;

            return result;
        }
    }
}