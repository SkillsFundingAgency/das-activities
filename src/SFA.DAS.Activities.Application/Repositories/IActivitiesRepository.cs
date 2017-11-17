using System.Collections.Generic;
using System.Threading.Tasks;
using NuGet;

namespace SFA.DAS.Activities.Application.Repositories
{
    public interface IActivitiesRepository
    {
        Task<Activity> GetActivity(Activity activity);

        Task SaveActivity(Activity activity);
    }
}
