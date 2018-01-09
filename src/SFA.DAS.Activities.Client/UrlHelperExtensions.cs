using System.Web.Mvc;
using SFA.DAS.Activities.Extensions;

namespace SFA.DAS.Activities.Client
{
    public static class UrlHelperExtensions
    {
        public static string GetActivitiesUrl(this UrlHelper urlHelper, long? total = null)
        {
            var url = urlHelper.Action("Activity", "EmployerTeam", new { take = total });

            return url;
        }

        public static string GetActivityUrl(this UrlHelper urlHelper, Activity activity)
        {
            var action = activity.Type.GetAction();
            var url = action != null ? urlHelper.Action(action.Item1, action.Item2) : null;

            return url;
        }
    }
}