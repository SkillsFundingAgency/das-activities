using System.Web.Mvc;
using SFA.DAS.Activities.Extensions;

namespace SFA.DAS.Activities.Client
{
    public static class UrlHelperExtensions
    {
        public static string Activities(this UrlHelper urlHelper, long? take = null)
        {
            var url = urlHelper.Action("Index", "Activities", new { take });

            return url;
        }

        public static string Activity(this UrlHelper urlHelper, Activity activity)
        {
            var action = activity.Type.GetAction();
            var url = action != null ? urlHelper.Action(action.Item1, action.Item2) : null;

            return url;
        }
    }
}