
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
    public class ProgressController : Controller
    {
        private readonly IProgressService _progressService;

        public ProgressController()
        {
            _progressService = new ProgressService();
        }

        // GET: /Client/Progress
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var model = await LoadProgressDataAsync(
                new ClientHealthMetricInputViewModel());

            return View(model);
        }

        // POST: /Client/Progress/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Prefix = "NewMetric")]
            ClientHealthMetricInputViewModel input)
        {
            if (input == null)
            {
                ModelState.AddModelError(
                    "",
                    "Please enter your health measurements.");

                input = new ClientHealthMetricInputViewModel();
            }

            if (!ModelState.IsValid)
            {
                var invalidModel =
                    await LoadProgressDataAsync(input);

                return View("Index", invalidModel);
            }

            string userId = User.Identity.GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new HttpUnauthorizedResult();
            }

            var savedMetric =
                await _progressService.AddHealthMetricAsync(
                    userId,
                    input.WeightKg,
                    input.HeightCm,
                    input.CaloriesBurned,
                    input.Steps,
                    input.SleepHours,
                    input.WaterLitres
                );

            if (savedMetric == null)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to save your measurements. " +
                    "Please check your client profile and try again.");

                var errorModel =
                    await LoadProgressDataAsync(input);

                return View("Index", errorModel);
            }

            TempData["ProgressSuccess"] =
                "Your health measurements have been saved successfully.";

            return RedirectToAction("Index");
        }

        // Shared page data loader.
        // Keeps the dashboard populated even if validation fails.
        private async Task<ClientProgressViewModel>
            LoadProgressDataAsync(
                ClientHealthMetricInputViewModel input)
        {
            string userId = User.Identity.GetUserId();

            var model = new ClientProgressViewModel
            {
                NewMetric = input ??
                    new ClientHealthMetricInputViewModel()
            };

            if (string.IsNullOrWhiteSpace(userId))
            {
                return model;
            }

            model.Metrics = await _progressService
                .GetHealthMetricsAsync(userId);

            model.Goals = await _progressService
                .GetGoalsAsync(userId);

            model.LatestMetric = await _progressService
                .GetLatestHealthMetricAsync(userId);

            return model;
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
