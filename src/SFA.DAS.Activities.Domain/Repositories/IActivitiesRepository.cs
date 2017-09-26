using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Activities.API.Types.Enums;
using SFA.DAS.Activities.Domain.Models;

namespace SFA.DAS.Activities.Domain.Repositories
{
    public interface IActivitiesRepository
    {
        Task<IEnumerable<Activity>> GetActivity(string ownerId);

        Task<Activity> GetActivity(string ownerId, ActivityType type);

        Task SaveActivity(Activity task);
    }
}
