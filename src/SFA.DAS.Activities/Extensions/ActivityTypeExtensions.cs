using System.Text.RegularExpressions;

namespace SFA.DAS.Activities.Extensions
{
    public static class ActivityTypeExtensions
    {
        public static string GetDescription(this ActivityType type)
        {
            return Regex.Replace(type.ToString(), "([A-Z])", " $1").Trim();
        }
    }
}