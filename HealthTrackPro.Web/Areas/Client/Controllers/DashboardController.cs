
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

            // Retrieve the client's profile
            var profile = await _clientService
                .GetProfileAsync(userId);

            // Retrieve current BMI
            var bmi = await _progressService
                .GetCurrentBmiAsync(userId);

            // Retrieve saved fitness goals
            var goals = await _progressService
                .GetGoalsAsync(userId);

            // Retrieve historical health measurements
            var healthMetrics = await _progressService
                .GetHealthMetricsAsync(userId);

            // Calculate fitness goal statistics
            decimal? averageProgress = null;
            int totalGoals = 0;
            int completedGoals = 0;

            if (goals != null && goals.Count > 0)
            {
                totalGoals = goals.Count;

                completedGoals = goals.Count(g =>
                    g.IsCompleted);

                averageProgress = Math.Round(
                    goals.Average(g => g.CalculateProgress()),
                    1
                );
            }

            // Prepare the Dashboard ViewModel
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

            // Prepare the latest 10 historical measurements.
            // The service returns newest records first.
            // Charts must display oldest to newest.
            if (healthMetrics != null)
            {
                model.HealthTrend = healthMetrics
                    .Where(m =>
                        m.WeightKg.HasValue ||
                        m.CalculateBMI().HasValue)
                    .OrderByDescending(m => m.RecordedAt)
                    .ThenByDescending(m => m.HealthMetricId)
                    .Take(10)
                    .OrderBy(m => m.RecordedAt)
                    .ThenBy(m => m.HealthMetricId)
                    .Select(m =>
                        new ClientHealthTrendPointViewModel
                        {
                            RecordedAt = m.RecordedAt,
                            WeightKg = m.WeightKg,
                            BMI = m.CalculateBMI()
                        })
                    .ToList();
            }

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
