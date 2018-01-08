using System;

namespace SFA.DAS.Activities.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToGmtStandardTime(this DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time"));
        }

        public static string ToRelativeFormat(this DateTime dateTime, DateTime now)
        {
            var date = dateTime.Date;
            var today = now.Date;
            var yesterday = today.AddDays(-1);

            if (date == today)
            {
                return "Today";
            }

            if (date == yesterday)
            {
                return "Yesterday";
            }

            var formattedDate = date.ToString("doo MMM yyyy");
            var day = date.Day;
            var remainder = day < 30 ? day % 20 : day % 30;
            var suffixes = new[] { "th", "st", "nd", "rd" };
            var suffix = remainder <= 3 ? suffixes[remainder] : suffixes[0];
            var result = formattedDate.Replace("oo", suffix);

            return result;
        }
    }
}