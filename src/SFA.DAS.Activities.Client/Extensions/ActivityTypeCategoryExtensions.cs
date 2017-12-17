using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SFA.DAS.Activities.Client.Attributes;

namespace SFA.DAS.Activities.Client.Extensions
{
    public static class ActivityTypeCategoryExtensions
    {
        public static List<ActivityType> GetActivityTypes(this ActivityTypeCategory category)
        {
            return Enum.GetValues(typeof(ActivityType))
                .Cast<ActivityType>()
                .Where(t => t.GetType().GetField(t.ToString()).GetCustomAttribute<CategoryAttribute>().Category == category)
                .ToList();
        }
    }
}