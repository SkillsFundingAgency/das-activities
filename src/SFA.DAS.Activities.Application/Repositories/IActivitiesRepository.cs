using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Activities.Application.Repositories
{
    public interface IActivitiesRepository
    {
        Task<IEnumerable<Activity>> GetActivities(string ownerId);

        Task<Activity> GetActivity(Activity activity);

        Task SaveActivity(Activity activity);
    }
}
