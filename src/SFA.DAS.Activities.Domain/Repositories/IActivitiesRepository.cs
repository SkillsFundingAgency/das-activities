using System.Collections.Generic;
using System.Threading.Tasks;
using NuGet;

namespace SFA.DAS.Activities.Domain.Repositories
{
    public interface IActivitiesRepository
    {
        Task<IEnumerable<Activity>> GetActivities(string ownerId);

        Task<Activity> GetActivity(Activity activity);

        Task SaveActivity(Activity activity);
    }
}
