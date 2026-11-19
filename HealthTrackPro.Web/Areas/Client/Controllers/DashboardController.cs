
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
        private readonly IWorkoutService _workoutService;

        public DashboardController()
        {
            _clientService = new ClientService();
            _progressService = new ProgressService();
            _workoutService = new WorkoutService();
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

            // HT-VIVA-DASHBOARD: existing active workout and recorded calorie data.
            var activeWorkoutPlan = await _workoutService.GetActivePlanAsync(userId);
            var workoutHistory = await _workoutService.GetWorkoutHistoryAsync(userId);
            int? caloriesBurnedToday = null;
            var today = DateTime.Today;

            // Prefer completed workout logs; use a health-metric entry only when
            // there are no calories recorded in today's completed workouts.
            if (workoutHistory != null)
            {
                var loggedToday = workoutHistory
                    .Where(w => w.IsCompleted && w.CompletedAt.HasValue &&
                        w.CompletedAt.Value.Date == today && w.CaloriesBurned.HasValue)
                    .ToList();
                if (loggedToday.Count > 0)
                    caloriesBurnedToday = loggedToday.Sum(w => w.CaloriesBurned.Value);
            }
            if (!caloriesBurnedToday.HasValue && healthMetrics != null)
            {
                var latestRecordedCalories = healthMetrics
                    .Where(m => m.RecordedAt.Date == today && m.CaloriesBurned.HasValue)
                    .OrderByDescending(m => m.RecordedAt)
                    .FirstOrDefault();
                if (latestRecordedCalories != null)
                    caloriesBurnedToday = latestRecordedCalories.CaloriesBurned;
            }

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

                CompletedGoals = completedGoals,
                CaloriesBurnedToday = caloriesBurnedToday,
                ActiveWorkoutName = activeWorkoutPlan != null ? activeWorkoutPlan.Name : null,
                ActiveWorkoutDescription = activeWorkoutPlan != null ? activeWorkoutPlan.Description : null,
                ActiveWorkoutTrainer = activeWorkoutPlan != null && activeWorkoutPlan.Trainer != null
                    ? activeWorkoutPlan.Trainer.FullName : null,
                ActiveWorkoutExerciseCount = activeWorkoutPlan != null && activeWorkoutPlan.WorkoutExercises != null
                    ? activeWorkoutPlan.WorkoutExercises.Count : 0
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

            if (disposing) (_workoutService as IDisposable)?.Dispose();
            base.Dispose(disposing);
        }
    }
}
