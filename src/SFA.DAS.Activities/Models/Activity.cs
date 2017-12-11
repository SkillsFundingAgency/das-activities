using System;
using Nest;

namespace SFA.DAS.Activities.Models
{
    public class Activity
    {
        [Keyword]
        public long AccountId { get; set; }

        [Keyword]
        public ActivityTypeEnum Type { get; set; }

        [Date]
        public DateTime At { get;  set; }

        [Text]
        public string CreatorName { get; set; }

        [Keyword]
        public string CreatorUserRef { get; set; }

        [Text]
        public string PayeScheme { get; set; }

        [Keyword]
        public long? ProviderUkprn { get; set; }
    }
}
