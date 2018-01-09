using System.Collections.Generic;

namespace SFA.DAS.Activities.Client
{
    public class ActivitiesResult
    {
        public IEnumerable<Activity> Activities { get; set; }
        public long Total { get; set; }

        public ActivitiesResult()
        {
            Activities = new List<Activity>();
        }
    }
}