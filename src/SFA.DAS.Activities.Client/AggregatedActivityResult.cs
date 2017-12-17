using SFA.DAS.Activities;

namespace SFA.DAS.Activities.Client
{
    public class AggregatedActivityResult
    {
        public Activity TopHit { get; set; }
        public long Count { get; set; }
    }
}