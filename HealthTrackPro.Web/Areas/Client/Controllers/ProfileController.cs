
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using HealthTrackPro.Web.Areas.Client.Models;
using HealthTrackPro.Web.Services.Interfaces;
using HealthTrackPro.Web.Services.Implementations;

namespace HealthTrackPro.Web.Areas.Client.Controllers
{
    [Authorize(Roles = "Client")]
    public class ProfileController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IProgressService _progressService;

        public ProfileController()
        {
            _clientService = new ClientService();
            _progressService = new ProgressService();
        }

        // GET: /Client/Profile
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string userId = User.Identity.GetUserId();

            var profile = await _clientService
                .GetProfileAsync(userId);

            if (profile == null)
            {
                return HttpNotFound();
            }

            var model = new ClientProfileViewModel
            {
                FullName = profile.FullName,
                DateOfBirth = profile.DateOfBirth,
                HeightCm = profile.HeightCm,
                CurrentWeightKg = profile.CurrentWeightKg,
                ActivityLevel = profile.ActivityLevel,
                PrimaryGoal = profile.PrimaryGoal
            };

            return View(model);
        }

        // POST: /Client/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(
            ClientProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.DateOfBirth.HasValue &&
                model.DateOfBirth.Value.Date > DateTime.Today)
            {
                ModelState.AddModelError(
                    "DateOfBirth",
                    "Date of birth cannot be in the future.");

                return View(model);
            }

            string userId = User.Identity.GetUserId();

            // Update profile through the shared service
            bool updated = await _clientService
                .UpdateProfileAsync(
                    userId,
                    model.FullName,
                    model.DateOfBirth,
                    model.HeightCm,
                    model.CurrentWeightKg,
                    model.ActivityLevel,
                    model.PrimaryGoal
                );

            if (!updated)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to update your profile.");

                return View(model);
            }

            // Record a health measurement when both
            // height and weight are available.
            if (model.HeightCm.HasValue &&
                model.CurrentWeightKg.HasValue)
            {
                var latest = await _progressService
                    .GetLatestHealthMetricAsync(userId);

                // Avoid recording duplicate measurements
                if (latest == null ||
                    latest.WeightKg != model.CurrentWeightKg ||
                    latest.HeightCm != model.HeightCm)
                {
                    var metric = await _progressService
                        .AddHealthMetricAsync(
                            userId,
                            model.CurrentWeightKg,
                            model.HeightCm,
                            null,
                            null,
                            null,
                            null
                        );

                    if (metric == null)
                    {
                        TempData["WarningMessage"] =
                            "Profile saved, but health measurement was not recorded.";
                    }
                }
            }

            TempData["SuccessMessage"] =
                "Profile updated successfully!";

            return RedirectToAction("Index");
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
