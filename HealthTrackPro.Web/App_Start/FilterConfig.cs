using System.Web;
using System.Web.Mvc;
using HealthTrackPro.Web.Filters;

namespace HealthTrackPro.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(
                new RequirePasswordChangeFilter());
        }
    }
}
