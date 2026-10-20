
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
    public class MealLogController : Controller
    {
        private readonly IDietService _dietService;

        public MealLogController()
        {
            _dietService = new DietService();
        }

        // GET: /Client/MealLog
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string userId = User.Identity.GetUserId();

            var logs = await _dietService
                .GetMealLogsAsync(userId);

            var activePlan = await _dietService
                .GetActivePlanAsync(userId);

            ViewBag.ActivePlan = activePlan;
            ViewBag.TodayCalories = logs
                .Where(m => m.LoggedAt.Date == DateTime.UtcNow.Date)
                .Sum(m => m.Calories);

            ViewBag.TodayProtein = logs
                .Where(m => m.LoggedAt.Date == DateTime.UtcNow.Date)
                .Sum(m => m.ProteinGrams ?? 0);

            ViewBag.TodayCarbs = logs
                .Where(m => m.LoggedAt.Date == DateTime.UtcNow.Date)
                .Sum(m => m.CarbohydrateGrams ?? 0);

            ViewBag.TodayFat = logs
                .Where(m => m.LoggedAt.Date == DateTime.UtcNow.Date)
                .Sum(m => m.FatGrams ?? 0);

            return View(logs);
        }

        // POST: /Client/MealLog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            ClientMealLogInputViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["MealLogError"] =
                    "Please check your meal details and try again.";

                return RedirectToAction("Index");
            }

            string userId = User.Identity.GetUserId();

            var activePlan = await _dietService
                .GetActivePlanAsync(userId);

            int? dietPlanId = activePlan != null
                ? (int?)activePlan.DietPlanId
                : null;

            var savedMeal = await _dietService.LogMealAsync(
                userId,
                dietPlanId,
                model.MealType,
                model.FoodName,
                model.Description,
                model.Calories.Value,
                model.ProteinGrams,
                model.CarbohydrateGrams,
                model.FatGrams
            );

            if (savedMeal == null)
            {
                TempData["MealLogError"] =
                    "Unable to save your meal. Please try again.";
            }
            else
            {
                TempData["MealLogSuccess"] =
                    "Your meal has been logged successfully.";
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposable = _dietService as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
