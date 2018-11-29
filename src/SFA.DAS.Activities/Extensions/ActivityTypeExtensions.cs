using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using SFA.DAS.Activities.Attributes;
using SFA.DAS.Activities.Localizers;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.Extensions
{
    public static class ActivityTypeExtensions
    {
        private static readonly Dictionary<ActivityType, Tuple<string, string>> ActionCache = Enum
            .GetValues(typeof(ActivityType))
            .Cast<ActivityType>()
            .Select(t => new
            {
                Type = t,
                Attribute = t.GetType()
                    .GetField(t.ToString())
                    .GetCustomAttribute<ActionAttribute>()
            })
            .ToDictionary(x => x.Type, x => x.Attribute == null ? null : new Tuple<string, string>(x.Attribute.Action, x.Attribute.Controller));

        private static readonly Dictionary<ActivityType, IActivityLocalizer> LocalizerCache = Enum
            .GetValues(typeof(ActivityType))
            .Cast<ActivityType>()
            .Select(t => new
            {
                Type = t,
                Attribute = t.GetType()
                    .GetField(t.ToString())
                    .GetCustomAttribute<LocalizerAttribute>()
            })
            .ToDictionary(x => x.Type, x => (IActivityLocalizer)Activator.CreateInstance(x.Attribute.Type));

        private static readonly Regex Regex = new Regex("([A-Z])", RegexOptions.Compiled);

        public static Tuple<string, string> GetAction(this ActivityType type)
        {
            return ActionCache[type];
        }

        public static string GetDescription(this ActivityType type)
        {
            return Regex.Replace(type.ToString(), " $1").Trim();
        }

        public static IActivityLocalizer GetLocalizer(this ActivityType type)
        {
            return LocalizerCache[type];
        }
    }
}