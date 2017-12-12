using System;
using System.Threading.Tasks;

namespace SFA.DAS.Activities.Client
{
    public interface IActivitiesClient
    {
        Task<ActivitiesResult> GetActivities(long accountId, int? take = null, DateTime? from = null, DateTime? to = null);
        Task<AggregatedActivitiesResult> GetLatestActivities(long accountId);
    }
}