using System;
using System.Collections.Generic;

namespace SFA.DAS.Activities
{
    public class Activity
    {
        public string Id { get; set; }
        public long AccountId { get; set; }
        public DateTime At { get; set; }
        public DateTime Created { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public string Description { get; set; }
        public ActivityType Type { get; set; }
    }
}