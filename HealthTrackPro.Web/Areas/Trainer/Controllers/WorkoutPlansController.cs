using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using HealthTrackPro.Web.Areas.Trainer.Models;
using HealthTrackPro.Web.Models.Profiles;
using HealthTrackPro.Web.Models.Workouts;
using HealthTrackPro.Web.Services.Implementations;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Areas.Trainer.Controllers
{
    /// <summary>Workout plans the logged-in trainer creates for assigned clients.</summary>
    [Authorize(Roles = "Trainer")]
    public class WorkoutPlansController : Controller
    {
        private readonly ITrainerService _trainerService;
        private readonly IWorkoutService _workoutService;

        public WorkoutPlansController()
        {
            _trainerService = new TrainerService();
            _workoutService = new WorkoutService();
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string trainerUserId = User.Identity.GetUserId();

            var plans = await _workoutService.GetTrainerPlansAsync(trainerUserId);
            var clients = await _trainerService.GetAssignedClientsAsync(trainerUserId);
            var names = clients.ToDictionary(c => c.UserId, c => c.FullName);

            var model = new WorkoutPlanIndexViewModel();
            foreach (WorkoutPlan plan in plans.OrderByDescending(p => p.StartDate))
            {
                model.Plans.Add(new WorkoutPlanRowViewModel
                {
                    WorkoutPlanId = plan.WorkoutPlanId,
                    ClientName = names.ContainsKey(plan.ClientUserId) ? names[plan.ClientUserId] : "Client no longer assigned",
                    PlanName = plan.Name,
                    Dates = plan.StartDate.ToString("MMM d, yyyy") +
                            (plan.EndDate.HasValue ? " to " + plan.EndDate.Value.ToString("MMM d, yyyy") : ""),
                    IsActive = plan.IsActive
                });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var model = new WorkoutPlanCreateViewModel();
            await LoadClientsAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(WorkoutPlanCreateViewModel model)
        {
            string trainerUserId = User.Identity.GetUserId();

            if (model.EndDate.HasValue && model.EndDate.Value < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date cannot be before the start date.");
            }

            if (!ModelState.IsValid)
            {
                await LoadClientsAsync(model);
                return View(model);
            }

            // CreatePlanAsync returns null unless the client is assigned to this trainer.
            WorkoutPlan plan = await _workoutService.CreatePlanAsync(
                model.ClientUserId,
                trainerUserId,
                model.Name.Trim(),
                model.Description,
                model.StartDate,
                model.EndDate);

            if (plan == null)
            {
                ModelState.AddModelError("ClientUserId", "This client is not assigned to you.");
                await LoadClientsAsync(model);
                return View(model);
            }

            TempData["SuccessMessage"] = "Workout plan created.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetActive(int id, bool isActive)
        {
            string trainerUserId = User.Identity.GetUserId();

            // The service only changes plans that belong to this trainer.
            bool changed = await _workoutService.SetPlanActiveAsync(id, trainerUserId, isActive);

            TempData[changed ? "SuccessMessage" : "ErrorMessage"] = changed
                ? (isActive ? "Plan activated." : "Plan deactivated.")
                : "That plan could not be updated.";

            return RedirectToAction("Index");
        }

        private async Task LoadClientsAsync(WorkoutPlanCreateViewModel model)
        {
            var clients = await _trainerService.GetAssignedClientsAsync(User.Identity.GetUserId());
            model.Clients = clients
                .Select(c => new SelectListItem { Value = c.UserId, Text = c.FullName })
                .ToList();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                (_trainerService as IDisposable)?.Dispose();
                (_workoutService as IDisposable)?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
