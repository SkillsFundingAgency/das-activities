using System;
using System.Threading.Tasks;
using SFA.DAS.Activities.IntegrityChecker.Dto;
using SFA.DAS.Activities.IntegrityChecker.Repositories;

namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{

    public interface IActivityDocumentRepository
    {
	    Task UpsertActivityAsync(Activity activity);
		Task<Activity[]> GetActivitiesAsync(IPagingData pagingData);
        Task<Activity> GetActivityAsync(Guid messageId);
        Task DeleteActivityAsync(Guid messageId);
        Task DeleteAllActivitiesFromRepoAsync();
    }
}