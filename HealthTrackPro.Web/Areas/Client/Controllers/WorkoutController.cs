
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
    public class WorkoutController : Controller
    {
        private readonly IWorkoutService _workoutService;

        public WorkoutController()
        {
            _workoutService = new WorkoutService();
        }

        // Existing Workout Plan page
        // GET: /Client/Workout
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string userId = User.Identity.GetUserId();

            var plans = await _workoutService
                .GetClientPlansAsync(userId);

            var activePlan = await _workoutService
                .GetActivePlanAsync(userId);

            var model = new ClientWorkoutViewModel
            {
                Plans = plans,
                ActivePlan = activePlan
            };

            return View(model);
        }

        // GET: /Client/Workout/History
        [HttpGet]
        public async Task<ActionResult> History()
        {
            string userId = User.Identity.GetUserId();

            var history = await _workoutService
                .GetWorkoutHistoryAsync(userId);

            var activePlan = await _workoutService
                .GetActivePlanAsync(userId);

            ViewBag.ActivePlan = activePlan;

            ViewBag.OngoingWorkout = history
                .FirstOrDefault(w => !w.IsCompleted);

            return View(history);
        }

        // POST: /Client/Workout/Start
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Start(
            int workoutPlanId,
            string notes)
        {
            string userId = User.Identity.GetUserId();

            // Prevent starting another workout while
            // a previous session remains unfinished.
            var history = await _workoutService
                .GetWorkoutHistoryAsync(userId);

            if (history.Any(w => !w.IsCompleted))
            {
                TempData["WorkoutError"] =
                    "You already have an ongoing workout. " +
                    "Complete it before starting another.";

                return RedirectToAction("History");
            }

            // Only allow the client's assigned active plan.
            var activePlan = await _workoutService
                .GetActivePlanAsync(userId);

            if (activePlan == null ||
                activePlan.WorkoutPlanId != workoutPlanId)
            {
                TempData["WorkoutError"] =
                    "No valid active workout plan was found.";

                return RedirectToAction("History");
            }

            if (notes != null && notes.Length > 500)
            {
                TempData["WorkoutError"] =
                    "Workout notes cannot exceed 500 characters.";

                return RedirectToAction("History");
            }

            var workout = await _workoutService
                .StartWorkoutAsync(
                    userId,
                    workoutPlanId,
                    notes
                );

            if (workout == null)
            {
                TempData["WorkoutError"] =
                    "Unable to start the workout.";
            }
            else
            {
                TempData["WorkoutSuccess"] =
                    "Workout started successfully!";
            }

            return RedirectToAction("History");
        }

        // POST: /Client/Workout/Complete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Complete(
            int workoutLogId,
            int? caloriesBurned)
        {
            string userId = User.Identity.GetUserId();

            if (caloriesBurned.HasValue &&
                (caloriesBurned.Value < 0 ||
                 caloriesBurned.Value > 5000))
            {
                TempData["WorkoutError"] =
                    "Calories burned must be between 0 and 5000.";

                return RedirectToAction("History");
            }

            // Confirm the workout belongs to the
            // currently signed-in client.
            var history = await _workoutService
                .GetWorkoutHistoryAsync(userId);

            var workout = history.FirstOrDefault(
                w => w.WorkoutLogId == workoutLogId
            );

            if (workout == null)
            {
                TempData["WorkoutError"] =
                    "Workout session was not found.";

                return RedirectToAction("History");
            }

            if (workout.IsCompleted)
            {
                TempData["WorkoutError"] =
                    "This workout is already completed.";

                return RedirectToAction("History");
            }

            bool completed = await _workoutService
                .CompleteWorkoutAsync(
                    userId,
                    workoutLogId,
                    caloriesBurned
                );

            if (completed)
            {
                TempData["WorkoutSuccess"] =
                    "Great work! Your session was completed " +
                    "and added to your workout history.";
            }
            else
            {
                TempData["WorkoutError"] =
                    "Unable to complete the workout.";
            }

            return RedirectToAction("History");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposable =
                    _workoutService as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
