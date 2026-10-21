using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using HealthTrackPro.Web.Areas.Trainer.Models;
using HealthTrackPro.Web.Models.Diet;
using HealthTrackPro.Web.Services.Implementations;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Areas.Trainer.Controllers
{
    /// <summary>Diet plans the logged-in trainer creates for assigned clients.</summary>
    [Authorize(Roles = "Trainer")]
    public class DietPlansController : Controller
    {
        private readonly ITrainerService _trainerService;
        private readonly IDietService _dietService;

        public DietPlansController()
        {
            _trainerService = new TrainerService();
            _dietService = new DietService();
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string trainerUserId = User.Identity.GetUserId();

            var plans = await _dietService.GetTrainerPlansAsync(trainerUserId);
            var clients = await _trainerService.GetAssignedClientsAsync(trainerUserId);
            var names = clients.ToDictionary(c => c.UserId, c => c.FullName);

            var model = new DietPlanIndexViewModel();
            foreach (DietPlan plan in plans.OrderByDescending(p => p.StartDate))
            {
                model.Plans.Add(new DietPlanRowViewModel
                {
                    DietPlanId = plan.DietPlanId,
                    ClientName = names.ContainsKey(plan.ClientUserId) ? names[plan.ClientUserId] : "Client no longer assigned",
                    PlanName = plan.Name,
                    Dates = plan.StartDate.ToString("MMM d, yyyy") +
                            (plan.EndDate.HasValue ? " to " + plan.EndDate.Value.ToString("MMM d, yyyy") : ""),
                    Targets = plan.DailyCaloriesTarget.HasValue
                        ? plan.DailyCaloriesTarget.Value + " kcal/day"
                        : "No calorie target",
                    IsActive = plan.IsActive
                });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var model = new DietPlanCreateViewModel();
            await LoadClientsAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DietPlanCreateViewModel model)
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
            DietPlan plan = await _dietService.CreatePlanAsync(
                model.ClientUserId,
                trainerUserId,
                model.Name.Trim(),
                model.Description,
                model.DailyCaloriesTarget,
                model.ProteinTargetGrams,
                model.CarbohydrateTargetGrams,
                model.FatTargetGrams,
                model.StartDate,
                model.EndDate);

            if (plan == null)
            {
                ModelState.AddModelError("ClientUserId", "This client is not assigned to you.");
                await LoadClientsAsync(model);
                return View(model);
            }

            TempData["SuccessMessage"] = "Diet plan created.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetActive(int id, bool isActive)
        {
            string trainerUserId = User.Identity.GetUserId();

            // The service only changes plans that belong to this trainer.
            bool changed = await _dietService.SetPlanActiveAsync(id, trainerUserId, isActive);

            TempData[changed ? "SuccessMessage" : "ErrorMessage"] = changed
                ? (isActive ? "Plan activated." : "Plan deactivated.")
                : "That plan could not be updated.";

            return RedirectToAction("Index");
        }

        private async Task LoadClientsAsync(DietPlanCreateViewModel model)
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
                (_dietService as IDisposable)?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
