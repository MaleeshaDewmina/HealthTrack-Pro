using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using System.Web.Routing;

namespace HealthTrackPro.Web.Filters
{
    public class RequirePasswordChangeFilter
        : ActionFilterAttribute
    {
        public override void OnActionExecuting(
            ActionExecutingContext filterContext)
        {
            if (filterContext == null ||
                filterContext.HttpContext == null ||
                filterContext.HttpContext.User == null ||
                filterContext.HttpContext.User.Identity == null ||
                !filterContext.HttpContext
                    .User.Identity.IsAuthenticated)
            {
                base.OnActionExecuting(
                    filterContext);

                return;
            }

            var identity =
                filterContext.HttpContext
                    .User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                base.OnActionExecuting(
                    filterContext);

                return;
            }

            bool mustChangePassword =
                identity.Claims.Any(c =>
                    c.Type ==
                        "MustChangePassword" &&
                    string.Equals(
                        c.Value,
                        "true",
                        StringComparison
                            .OrdinalIgnoreCase));

            if (!mustChangePassword)
            {
                base.OnActionExecuting(
                    filterContext);

                return;
            }

            string controller =
                Convert.ToString(
                    filterContext.RouteData
                        .Values["controller"]);

            string action =
                Convert.ToString(
                    filterContext.RouteData
                        .Values["action"]);

            bool changePasswordPage =
                string.Equals(
                    controller,
                    "Manage",
                    StringComparison
                        .OrdinalIgnoreCase) &&
                string.Equals(
                    action,
                    "ChangePassword",
                    StringComparison
                        .OrdinalIgnoreCase);

            bool logoutAction =
                string.Equals(
                    controller,
                    "Account",
                    StringComparison
                        .OrdinalIgnoreCase) &&
                string.Equals(
                    action,
                    "LogOff",
                    StringComparison
                        .OrdinalIgnoreCase);

            if (changePasswordPage ||
                logoutAction)
            {
                base.OnActionExecuting(
                    filterContext);

                return;
            }

            filterContext.Result =
                new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller =
                                "Manage",

                            action =
                                "ChangePassword",

                            area =
                                "",

                            required =
                                true
                        }));
        }
    }
}