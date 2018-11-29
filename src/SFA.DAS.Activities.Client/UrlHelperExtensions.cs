using System.Web.Mvc;
using System.Web.Routing;
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
            var link = activity.GetLinkDetails();
            
            if (link == null)
                return null;

            var routeValues = new RouteValueDictionary(link.Parameters);

            var url = urlHelper.Action(link.Action, link.Controller, routeValues);

            return url;
        }
    }
}