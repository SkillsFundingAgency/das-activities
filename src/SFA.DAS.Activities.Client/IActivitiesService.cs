using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Activities.Client
{
    public interface IActivitiesService
    {
        Task AddActivity(Activity activity);
        Task<IEnumerable<AggregatedActivity>> GetAggregatedActivities(long accountId);
    }
}