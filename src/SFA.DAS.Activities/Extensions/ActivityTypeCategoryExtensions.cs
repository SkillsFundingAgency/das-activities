﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SFA.DAS.Activities.Attributes;

namespace SFA.DAS.Activities.Extensions
{
    public static class ActivityTypeCategoryExtensions
    {
        private static readonly Dictionary<ActivityTypeCategory, List<ActivityType>> TypeCache = Enum
            .GetValues(typeof(ActivityTypeCategory))
            .Cast<ActivityTypeCategory>()
            .ToDictionary(c => c, c => Enum
                .GetValues(typeof(ActivityType))
                .Cast<ActivityType>()
                .Where(t => t
                    .GetType()
                    .GetField(t.ToString())
                    .GetCustomAttribute<CategoryAttribute>().Category == c)
                .ToList());

        public static List<ActivityType> GetActivityTypes(this ActivityTypeCategory category)
        {
            return TypeCache[category];
        }
    }
}