
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using HealthTrackPro.Web.Areas.Client.Models;
using HealthTrackPro.Web.Enums;
using HealthTrackPro.Web.Services.Interfaces;
using HealthTrackPro.Web.Services.Implementations;

namespace HealthTrackPro.Web.Areas.Client.Controllers
{
    [Authorize(Roles = "Client")]
    public class GoalsController : Controller
    {
        private readonly IProgressService _progressService;

        public GoalsController()
        {
            _progressService = new ProgressService();
        }

        // GET: /Client/Goals
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string userId = User.Identity.GetUserId();

            var goals = await _progressService
                .GetGoalsAsync(userId);

            var model = new ClientGoalsViewModel
            {
                Goals = goals
            };

            return View(model);
        }

        // POST: /Client/Goals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Prefix = "NewGoal")]
            CreateFitnessGoalViewModel model)
        {
            if (model == null)
            {
                ModelState.AddModelError(
                    "",
                    "Please enter your goal details.");

                model = new CreateFitnessGoalViewModel();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.Title))
                {
                    ModelState.AddModelError(
                        "NewGoal.Title",
                        "Please enter a goal title.");
                }

                if (model.GoalType.HasValue &&
                    !Enum.IsDefined(
                        typeof(FitnessGoalType),
                        model.GoalType.Value))
                {
                    ModelState.AddModelError(
                        "NewGoal.GoalType",
                        "Please select a valid goal type.");
                }

                if (model.StartValue.HasValue &&
                    model.TargetValue.HasValue &&
                    model.StartValue.Value ==
                        model.TargetValue.Value)
                {
                    ModelState.AddModelError(
                        "NewGoal.TargetValue",
                        "Your target must be different from your starting value.");
                }

                if (model.TargetDate.HasValue &&
                    model.TargetDate.Value.Date < DateTime.Today)
                {
                    ModelState.AddModelError(
                        "NewGoal.TargetDate",
                        "Target date cannot be in the past.");
                }
            }

            string userId = User.Identity.GetUserId();

            if (!ModelState.IsValid)
            {
                var goals = await _progressService
                    .GetGoalsAsync(userId);

                return View(
                    "Index",
                    new ClientGoalsViewModel
                    {
                        Goals = goals,
                        NewGoal = model
                    });
            }

            var goal = await _progressService
                .CreateGoalAsync(
                    userId,
                    model.GoalType.Value,
                    model.Title.Trim(),
                    model.Description,
                    model.StartValue,
                    model.TargetValue,
                    model.TargetDate
                );

            if (goal == null)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to create the fitness goal.");

                var goals = await _progressService
                    .GetGoalsAsync(userId);

                return View(
                    "Index",
                    new ClientGoalsViewModel
                    {
                        Goals = goals,
                        NewGoal = model
                    });
            }

            TempData["SuccessMessage"] =
                "Fitness goal created successfully!";

            return RedirectToAction("Index");
        }

        // POST: /Client/Goals/UpdateProgress
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateProgress(
            UpdateFitnessGoalValueViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] =
                    "Please enter a valid current value.";

                return RedirectToAction("Index");
            }

            string userId = User.Identity.GetUserId();

            bool updated = await _progressService
                .UpdateGoalValueAsync(
                    model.FitnessGoalId,
                    userId,
                    model.CurrentValue.Value
                );

            if (!updated)
            {
                TempData["ErrorMessage"] =
                    "Unable to update this goal.";
            }
            else
            {
                TempData["SuccessMessage"] =
                    "Goal progress updated successfully!";
            }

            return RedirectToAction("Index");
        }

        // POST: /Client/Goals/MarkCompleted
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MarkCompleted(
            int fitnessGoalId)
        {
            if (fitnessGoalId <= 0)
            {
                TempData["ErrorMessage"] =
                    "Invalid fitness goal.";

                return RedirectToAction("Index");
            }

            string userId = User.Identity.GetUserId();

            bool completed = await _progressService
                .MarkGoalCompletedAsync(
                    fitnessGoalId,
                    userId
                );

            if (!completed)
            {
                TempData["ErrorMessage"] =
                    "Unable to complete this goal.";
            }
            else
            {
                TempData["SuccessMessage"] =
                    "Fitness goal marked as completed!";
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposable =
                    _progressService as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
