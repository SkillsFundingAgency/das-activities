using System.Threading.Tasks;
using SFA.DAS.Activities.Domain.Models;

namespace SFA.DAS.Activities.Domain.Repositories
{
    public interface IActivitiesRepository
    {
        //Task<IEnumerable<Activity>> GetActivities(string ownerId);

        //Task<Activity> GetActivity(string ownerId, ActivityType type);

        Task<Activity> GetActivity(Activity message);

        Task SaveActivity(Activity activity);
    }
}
