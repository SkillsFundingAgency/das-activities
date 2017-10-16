using System.Collections.Generic;

namespace SFA.DAS.Activities.Application.Queries.GetActivities
{
    public class GetActivitiesByOwnerIdResponse
    {
        public IEnumerable<Activity> Activities { get; set; }
    }
}
