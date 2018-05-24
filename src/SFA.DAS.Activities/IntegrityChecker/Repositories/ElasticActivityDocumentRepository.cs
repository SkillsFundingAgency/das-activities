using System.Threading.Tasks;
using Nest;

namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{
    public class ElasticActivityDocumentRepository : IActivityDocumentRepository
    {
        private readonly IElasticClient _elasticClient;

        public ElasticActivityDocumentRepository(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public Task<ActivityPageResult> GetActivitiesAsync(int startPage, int pageSize)
        {
            return null;
        }
    }
}