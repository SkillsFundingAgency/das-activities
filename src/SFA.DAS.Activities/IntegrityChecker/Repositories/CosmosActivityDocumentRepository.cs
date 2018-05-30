using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
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

        public async Task<Activity[]> GetActivitiesAsync(IPagingData pagingData)
        {
            if (!(pagingData is CosmosPagingData cosmosPagingData))
            {
                throw new InvalidQueryPagingData(
                    $"paging data supplied to {nameof(CosmosActivityDocumentRepository)} is not a {nameof(CosmosPagingData)}");
            }

            var cosmosResults = await _cosmosClient.GetPage<Activity, string>(_config.CosmosCollectionName, cosmosPagingData.ContinuationToken, documentType => documentType.Id, pagingData.RequiredPageSize);

            cosmosPagingData.ContinuationToken = cosmosResults.ContinuationToken;

            return cosmosResults.Items;
        }
    }
}