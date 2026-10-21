using System.Web.Mvc;

namespace HealthTrackPro.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SettingsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}