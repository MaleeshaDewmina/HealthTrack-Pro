using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using HealthTrackPro.Web.Areas.Trainer.Models;
using HealthTrackPro.Web.Models.Fitness;
using HealthTrackPro.Web.Models.Profiles;
using HealthTrackPro.Web.Services.Implementations;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Areas.Trainer.Controllers
{
    /// <summary>Read-only progress summary for the trainer's assigned clients.</summary>
    [Authorize(Roles = "Trainer")]
    public class ProgressController : Controller
    {
        private readonly ITrainerService _trainerService;
        private readonly IProgressService _progressService;

        public ProgressController()
        {
            _trainerService = new TrainerService();
            _progressService = new ProgressService();
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string trainerUserId = User.Identity.GetUserId();
            var clients = await _trainerService.GetAssignedClientsAsync(trainerUserId);

            var model = new ProgressReportViewModel
            {
                GeneratedAt = DateTime.Now
            };

            foreach (ClientProfile client in clients.OrderBy(c => c.FullName))
            {
                var goals = await _progressService.GetGoalsAsync(client.UserId);
                var latest = await _progressService.GetLatestHealthMetricAsync(client.UserId);
                var bmi = await _progressService.GetCurrentBmiAsync(client.UserId);
                var active = goals.Where(g => !g.IsCompleted).ToList();

                model.Clients.Add(new ProgressClientRowViewModel
                {
                    ClientUserId = client.UserId,
                    ClientName = client.FullName,
                    Goal = client.PrimaryGoal.HasValue
                        ? SplitPascalCase(client.PrimaryGoal.Value.ToString())
                        : "Not set",
                    CurrentWeight = latest != null && latest.WeightKg.HasValue
                        ? latest.WeightKg.Value.ToString("0.0") + " kg"
                        : "No weight recorded",
                    LastRecorded = latest != null
                        ? latest.RecordedAt.ToString("MMM d, yyyy")
                        : "Never",
                    Bmi = bmi.HasValue ? bmi.Value.ToString("0.0") : "-",
                    ActiveGoals = active.Count,
                    CompletedGoals = goals.Count - active.Count,
                    AverageGoalProgress = active.Count == 0
                        ? "-"
                        : Math.Round(active.Average(g => g.CalculateProgress()), 0) + "%"
                });
            }

            return View(model);
        }

        private static string SplitPascalCase(string value)
        {
            return Regex.Replace(value, "([a-z])([A-Z])", "$1 $2");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                (_trainerService as IDisposable)?.Dispose();
                (_progressService as IDisposable)?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
