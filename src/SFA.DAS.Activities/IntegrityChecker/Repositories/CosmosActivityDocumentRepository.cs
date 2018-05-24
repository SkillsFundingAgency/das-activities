using System.Threading.Tasks;

namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{
    public class CosmosActivityDocumentRepository : IActivityDocumentRepository
    {
        private readonly ICosmosClient _cosmosClient;

        public CosmosActivityDocumentRepository(ICosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        public Task<ActivityPageResult> GetActivitiesAsync(int startPage, int pageSize)
        {
            return null;
        }
    }
}