using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using HealthTrackPro.Web.Areas.Trainer.Models;
using HealthTrackPro.Web.Models.Profiles;
using HealthTrackPro.Web.Services.Implementations;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Areas.Trainer.Controllers
{
    /// <summary>Trainer-only pages for the Clients assigned to the logged-in trainer.</summary>
    [Authorize(Roles = "Trainer")]
    public class ClientsController : Controller
    {
        private readonly ITrainerService _trainerService;
        private readonly IProgressService _progressService;
        private readonly IClientService _clientService;
        private readonly IWorkoutService _workoutService;
        private readonly IDietService _dietService;

        public ClientsController()
        {
            _trainerService = new TrainerService();
            _progressService = new ProgressService();
            _clientService = new ClientService();
            _workoutService = new WorkoutService();
            _dietService = new DietService();
        }

        /// <summary>Lists only the clients assigned to the signed-in trainer.</summary>
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            // The trainer comes from the signed-in identity, never from the URL or form.
            string trainerUserId = User.Identity.GetUserId();

            List<ClientProfile> clients =
                await _trainerService.GetAssignedClientsAsync(trainerUserId);

            var model = new TrainerClientListViewModel();

            foreach (ClientProfile client in clients)
            {
                decimal? bmi =
                    await _progressService.GetCurrentBmiAsync(client.UserId);

                model.Clients.Add(
                    TrainerClientListItemViewModel.FromProfile(client, bmi));
            }

            return View(model);
        }

        /// <summary>
        /// Shows one client's details. The client ID comes from the URL, so the
        /// assignment is checked before any data is loaded.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            string trainerUserId = User.Identity.GetUserId();

            bool isAssigned =
                await _trainerService.IsClientAssignedAsync(trainerUserId, id);

            if (!isAssigned)
            {
                // Not assigned to this trainer: refuse, and reveal nothing about the client.
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            ClientProfile profile = await _clientService.GetProfileAsync(id);
            if (profile == null)
            {
                return HttpNotFound();
            }

            decimal? bmi = await _progressService.GetCurrentBmiAsync(id);
            var goals = await _progressService.GetGoalsAsync(id);
            var metrics = await _progressService.GetHealthMetricsAsync(id);
            var workoutPlans = await _workoutService.GetClientPlansAsync(id);
            var dietPlans = await _dietService.GetClientPlansAsync(id);

            var model = new ClientDetailsViewModel
            {
                ClientUserId = profile.UserId,
                FullName = string.IsNullOrWhiteSpace(profile.FullName) ? "Unnamed client" : profile.FullName,
                PrimaryGoal = profile.PrimaryGoal.HasValue
                    ? System.Text.RegularExpressions.Regex.Replace(profile.PrimaryGoal.Value.ToString(), "(?<!^)([A-Z])", " $1")
                    : "Not set",
                CurrentWeightKg = profile.CurrentWeightKg,
                HeightCm = profile.HeightCm,
                Bmi = bmi
            };

            foreach (var goal in goals)
            {
                model.Goals.Add(new ClientGoalRowViewModel
                {
                    Title = goal.Title,
                    GoalType = goal.GoalType.ToString(),
                    CurrentValue = goal.CurrentValue,
                    TargetValue = goal.TargetValue,
                    ProgressPercent = Math.Round(goal.CalculateProgress(), 0),
                    IsCompleted = goal.IsCompleted
                });
            }

            foreach (var metric in metrics.OrderByDescending(m => m.RecordedAt).Take(10))
            {
                model.Measurements.Add(new ClientMeasurementRowViewModel
                {
                    RecordedAt = metric.RecordedAt,
                    WeightKg = metric.WeightKg,
                    Bmi = metric.CalculateBMI(),
                    Steps = metric.Steps
                });
            }

            // Only plans this trainer assigned, so a trainer never sees another trainer's plans here.
            foreach (var plan in workoutPlans.Where(p => p.TrainerUserId == trainerUserId))
            {
                model.WorkoutPlans.Add(new ClientPlanRowViewModel
                {
                    Name = plan.Name,
                    Detail = plan.StartDate.ToString("MMM d, yyyy") +
                             (plan.EndDate.HasValue ? " to " + plan.EndDate.Value.ToString("MMM d, yyyy") : ""),
                    IsActive = plan.IsActive
                });
            }

            foreach (var plan in dietPlans.Where(p => p.TrainerUserId == trainerUserId))
            {
                model.DietPlans.Add(new ClientPlanRowViewModel
                {
                    Name = plan.Name,
                    Detail = (plan.DailyCaloriesTarget.HasValue ? plan.DailyCaloriesTarget + " kcal/day" : "No calorie target"),
                    IsActive = plan.IsActive
                });
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                (_trainerService as IDisposable)?.Dispose();
                (_progressService as IDisposable)?.Dispose();
                (_clientService as IDisposable)?.Dispose();
                (_workoutService as IDisposable)?.Dispose();
                (_dietService as IDisposable)?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
