using System.Web.Mvc;

namespace HealthTrackPro.Web.Areas.Trainer.Controllers
{
    [Authorize(Roles = "Trainer")]
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
