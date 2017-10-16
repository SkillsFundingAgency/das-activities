using System;
using System.Globalization;

namespace SFA.DAS.Activities.Application
{
    public class Activity
    {
        public Activity()
        {
            
        }

        public Activity(string ownerId, ActivityType activityType, string description, string url, DateTime postedDate)
        {
            OwnerId = ownerId;
            ActivityType = activityType;
            Description = description;
            Url = url;
            PostedDateTime = postedDate;
        }

        public string OwnerId { get; internal set; }

        public ActivityType ActivityType { get; internal set; }

        public string Description { get; internal set; }

        public string Url { get; internal set; }

        public DateTime PostedDateTime { get; internal set; }
    }
}
