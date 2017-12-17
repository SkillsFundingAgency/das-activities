using System.Collections.Generic;
using SFA.DAS.Activities;

namespace SFA.DAS.Activities.Client
{
    public class ActivitiesResult
    {
        public IEnumerable<Activity> Activities { get; set; }
        public long Total { get; set; }
    }
}