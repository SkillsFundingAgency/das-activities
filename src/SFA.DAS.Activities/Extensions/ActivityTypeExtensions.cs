using System;
using System.Reflection;
using System.Text.RegularExpressions;
using SFA.DAS.Activities.Attributes;
using SFA.DAS.Activities.Localizers;

namespace SFA.DAS.Activities.Extensions
{
    public static class ActivityTypeExtensions
    {
        public static Tuple<string, string> GetAction(this ActivityType type)
        {
            var actionAttribute = type.GetType().GetField(type.ToString()).GetCustomAttribute<ActionAttribute>();

            return actionAttribute != null
                ? new Tuple<string, string>(actionAttribute.Action, actionAttribute.Controller)
                : null;
        }

        public static string GetDescription(this ActivityType type)
        {
            return Regex.Replace(type.ToString(), "([A-Z])", " $1").Trim();
        }

        public static IActivityLocalizer GetLocalizer(this ActivityType type)
        {
            var localizerAttribute = type.GetType().GetField(type.ToString()).GetCustomAttribute<LocalizerAttribute>();
            var localizer = (IActivityLocalizer)Activator.CreateInstance(localizerAttribute.Type);

            return localizer;
        }
    }
}