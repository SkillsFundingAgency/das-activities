using System.Threading.Tasks;
using SFA.DAS.Activities.IntegrityChecker.Dto;
using SFA.DAS.Activities.IntegrityChecker.Repositories;

namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{

    public interface IActivityDocumentRepository
    {
        Task<Activity[]> GetActivitiesAsync(IPagingData pagingData);
    }

    public interface ICosmosActivityDocumentRepository : IActivityDocumentRepository
    {

    }

    public interface IElasticActivityDocumentRepository : IActivityDocumentRepository
    {

    }
}