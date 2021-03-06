﻿using System;
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

        private static readonly ILog Log = new NLogLogger(typeof(ActivityTypeExtensions));

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

        public static string GetMessageForActivity(this Activity activity, string message, params string[] dataItems)
        {
            return activity.GetMessageForActivity(message, null, dataItems);
        }

        /// <summary>
        ///     Formats the supplied message using the data items available in the specified activity. If any
        ///     data items are not in the activity then a generic message will be returned <see cref="GetGenericMessage"/>.
        /// </summary>
        /// <param name="formatter">If supplied then this will be called after each value is obtained to allow the value to be custom formatted</param>
        /// <param name="dataItems">An array of data item names that are required in the activity's data collection.</param>
        /// <returns></returns>
        public static string GetMessageForActivity(
            this Activity activity, 
            string message, 
            Func<string, string, string> formatter, 
            params string[] dataItems)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            var values = dataItems
                .GetValuesUntilNotFound(activity, formatter)
                .Cast<object>()
                .ToArray();

            if (values.Length == dataItems.Length)
            {
                return string.Format(message, values);
            }

            return activity.GetGenericMessage();
        }

        public static string GetGenericMessage(this Activity activity)
        {
            return $"{activity.Description} occurred";
        }

        private static IEnumerable<string> GetValuesUntilNotFound(this IEnumerable<string> dataItems, Activity activity, Func<string, string, string> formatter)
        {
            foreach (var dataItem in dataItems)
            {
                if (activity.Data.TryGetValue(dataItem, out string dataValue))
                {
                    if (formatter != null)
                    {
                        dataValue = formatter(dataItem, dataValue);
                    }
                    yield return dataValue;
                }
                else
                {
                    Log.Warn($"Could not format activity message for activity {activity.Id} {activity.Type} on account {activity.AccountId} because the required data item \"{dataItem}\" is missing from the activity's data property. Generic message being used instead.");
                    yield break;
                }
            }
        }
    }
}