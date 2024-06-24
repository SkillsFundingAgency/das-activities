using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Activities.Models;
using SFA.DAS.NLog.Logger;


namespace SFA.DAS.Activities.Extensions
{
    public static class ActivityExtensions
    {
        private static readonly ILog Log = new NLogLogger(typeof(ActivityExtensions));

        public static ActivityUrlLink GetLinkDetails(this Activity activity)
        {
            var link = activity.Type.GetActivityLink();

            if (link == null)
                return null;

            if (activity.Type == ActivityType.PaymentCreated)
            {
                link.Parameters.Add("month", activity.At.Month);
                link.Parameters.Add("year", activity.At.Year);
            }

            return link;
        }

        public static string GetMessageForActivity(this Activity activity, string message, params string[] dataItems)
        {
            return activity.GetMessageForActivity(message, null, dataItems);
        }

        public static string GetGenericMessage(this Activity activity)
        {
            return $"{activity.Description} occurred";
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
