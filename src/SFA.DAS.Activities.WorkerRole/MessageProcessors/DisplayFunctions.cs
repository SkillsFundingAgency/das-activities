using System;

namespace SFA.DAS.Activities.WorkerRole.MessageProcessors
{
    public static class DisplayFunctions
    {
        public static string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("h:mm tt").ToLower();
        }

        public static string Format(this DateTime datetime)
        {
            return FormatDateTime(datetime);
        }
    }
}
