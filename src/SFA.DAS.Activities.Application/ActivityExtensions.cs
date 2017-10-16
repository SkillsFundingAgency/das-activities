using System;

namespace SFA.DAS.Activities.Application
{
    public static class ActivityExtensions
    {
        public static Activity WithOwnerId(this Activity activity, string ownerId)
        {
            activity.OwnerId = ownerId;
            return activity;
        }

        public static Activity WithActivityType(this Activity activity, ActivityType activityType)
        {
            activity.ActivityType = activityType;
            return activity;
        }

        public static Activity WithDescription(this Activity activity, string description)
        {
            activity.Description = description;
            return activity;
        }

        public static Activity WithUrl(this Activity activity, string url)
        {
            activity.Url = url;
            return activity;
        }

        public static Activity WithPostedDateTime(this Activity activity, DateTime postedDateTime)
        {
            activity.PostedDateTime = postedDateTime;
            return activity;
        }
    }
}
