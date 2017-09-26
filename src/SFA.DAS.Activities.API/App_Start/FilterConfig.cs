using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.Activities.API
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
