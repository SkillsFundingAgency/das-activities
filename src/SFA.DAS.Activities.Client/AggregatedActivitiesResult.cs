using System.Collections.Generic;

namespace SFA.DAS.Activities.Client
{
    public class AggregatedActivitiesResult
    {
        public IEnumerable<AggregatedActivityResult> Aggregates { get; set; }
        public long Total { get; set; }
    }
}