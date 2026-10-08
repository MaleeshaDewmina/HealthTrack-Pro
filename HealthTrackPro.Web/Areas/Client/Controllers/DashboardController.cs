
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using HealthTrackPro.Web.Areas.Client.Models;
using HealthTrackPro.Web.Services.Interfaces;
using HealthTrackPro.Web.Services.Implementations;

namespace HealthTrackPro.Web.Areas.Client.Controllers
{
    [Authorize(Roles = "Client")]
    public class DashboardController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IProgressService _progressService;

        public DashboardController()
        {
            _clientService = new ClientService();
            _progressService = new ProgressService();
        }

        public async Task<ActionResult> Index()
        {
            // Get the currently logged-in Client ID
            string userId = User.Identity.GetUserId();

            // Retrieve Client information using shared services
            var profile = await _clientService
                .GetProfileAsync(userId);

            var bmi = await _progressService
                .GetCurrentBmiAsync(userId);

            // Prepare data for the Dashboard
            var model = new ClientDashboardViewModel
            {
                FullName = profile != null
                    ? profile.FullName
                    : "Client",

                CurrentWeightKg = profile != null
                    ? profile.CurrentWeightKg
                    : null,

                HeightCm = profile != null
                    ? profile.HeightCm
                    : null,

                BMI = bmi
            };

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var clientDisposable =
                    _clientService as IDisposable;

                if (clientDisposable != null)
                {
                    clientDisposable.Dispose();
                }

                var progressDisposable =
                    _progressService as IDisposable;

                if (progressDisposable != null)
                {
                    progressDisposable.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
