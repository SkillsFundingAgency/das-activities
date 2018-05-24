using System.Threading.Tasks;

namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{
    public interface IActivityDocumentRepository
    {
        Task<ActivityPageResult> GetActivitiesAsync(int startPage, int pageSize);
    }
}