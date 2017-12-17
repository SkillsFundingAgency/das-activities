using System;
using System.Threading.Tasks;

namespace SFA.DAS.Activities.Client
{
    public interface IActivitiesClient
    {
        Task<ActivitiesResult> GetActivities(ActivitiesQuery query);
        Task<AggregatedActivitiesResult> GetLatestActivities(long accountId);
    }
}