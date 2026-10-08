
using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using HealthTrackPro.Web.Models;
using HealthTrackPro.Web.Areas.Client.Models;

namespace HealthTrackPro.Web.Areas.Client.Controllers
{
    [Authorize(Roles = "Client")]
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            var model = new ClientDashboardViewModel();

            // Get the currently logged-in Client
            string userId = User.Identity.GetUserId();

            using (var db = new ApplicationDbContext())
            {
                // Only retrieve this Client's profile
                var profile = db.ClientProfiles
                    .FirstOrDefault(p => p.UserId == userId);

                if (profile != null)
                {
                    model.FullName = profile.FullName;
                    model.CurrentWeightKg = profile.CurrentWeightKg;
                    model.HeightCm = profile.HeightCm;

                    // Calculate BMI when measurements exist
                    if (profile.CurrentWeightKg.HasValue &&
                        profile.HeightCm.HasValue &&
                        profile.HeightCm.Value > 0)
                    {
                        decimal heightMeters =
                            profile.HeightCm.Value / 100m;

                        model.BMI = Math.Round(
                            profile.CurrentWeightKg.Value /
                            (heightMeters * heightMeters),
                            1
                        );
                    }
                }
            }

            return View(model);
        }
    }
}
