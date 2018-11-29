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
            var link = activity.GetDetailsLink();
            
            if (link == null)
                return null;

            var url = urlHelper.Action(link.Action, link.Controller);

            return url;
        }
    }
}