using System;

namespace SFA.DAS.Activities.Client
{
    public class AggregatedActivity
    {
        public long AccountId { get; set; }
        public ActivityType Type { get; set; }
        public DateTime At { get; set; }
        public string CreatorName { get; set; }
        public string CreatorUserRef { get; set; }
        public string PayeScheme { get; set; }
        public long? ProviderUkprn { get; set; }
        public long Count { get; set; }
    }
}