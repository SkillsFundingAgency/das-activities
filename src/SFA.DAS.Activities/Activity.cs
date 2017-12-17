using System;
using System.Collections.Generic;
using Nest;

namespace SFA.DAS.Activities
{
    public class Activity
    {
        public long AccountId { get; set; }
        public DateTime At { get; set; }

        [Nested(IncludeInParent = true)]
        public Dictionary<string, string> Data { get; set; }

        public string Description { get; set; }
        public ActivityType Type { get; set; }
    }
}