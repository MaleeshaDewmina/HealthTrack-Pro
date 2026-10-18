
using System;
using System.Linq;
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

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            // Get the logged-in client's user ID
            string userId = User.Identity.GetUserId();

            // Retrieve client profile
            var profile = await _clientService
                .GetProfileAsync(userId);

            // Retrieve current BMI
            var bmi = await _progressService
                .GetCurrentBmiAsync(userId);

            // Retrieve fitness goals
            var goals = await _progressService
                .GetGoalsAsync(userId);

            // Calculate goal statistics
            decimal? averageProgress = null;
            int totalGoals = 0;
            int completedGoals = 0;

            if (goals != null && goals.Count > 0)
            {
                totalGoals = goals.Count;

                completedGoals = goals.Count(g =>
                    g.IsCompleted);

                // Calculate the average recorded progress.
                // A manually completed goal retains its
                // actual measured progress percentage.
                averageProgress = Math.Round(
                    goals.Average(g => g.CalculateProgress()),
                    1
                );
            }

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

                BMI = bmi,

                GoalProgressPercent = averageProgress,

                TotalGoals = totalGoals,

                CompletedGoals = completedGoals
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
