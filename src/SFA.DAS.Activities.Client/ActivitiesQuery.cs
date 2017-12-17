using System;
using System.Collections.Generic;
using SFA.DAS.Activities;

namespace SFA.DAS.Activities.Client
{
    public class ActivitiesQuery
    {
        public long AccountId { get; set; }
        public ActivityTypeCategory? Category { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public DateTime? From { get; set; }
        public string Term { get; set; }
        public int? Take { get; set; }
        public DateTime? To { get; set; }
    }
}