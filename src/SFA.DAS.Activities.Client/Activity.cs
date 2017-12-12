using System;
using System.Collections.Generic;
using Nest;

namespace SFA.DAS.Activities.Client
{
    public class Activity
    {
        [Keyword]
        public ActivityType Type { get; set; }

        [Keyword]
        public long AccountId { get; set; }

        [Date]
        public DateTime At { get; set; }

        [Nested]
        public IDictionary<string, string> Data { get; set; }

        [Object]
        public ICollection<string> Keywords { get; set; }
    }
}