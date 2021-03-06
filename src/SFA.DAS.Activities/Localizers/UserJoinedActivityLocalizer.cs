﻿using SFA.DAS.Activities.Extensions;

namespace SFA.DAS.Activities.Localizers
{
    public class UserJoinedActivityLocalizer : IActivityLocalizer
    {
        public string GetPluralText(Activity activity, long count)
        {
            return $"{count} users joined";
        }

        public string GetSingularText(Activity activity)
        {
            return activity.GetMessageForActivity("{0} joined", "CreatorName");
        }
    }
}