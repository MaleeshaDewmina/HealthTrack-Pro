using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using HealthTrackPro.Web.Areas.Trainer.Models;
using HealthTrackPro.Web.Models;
using HealthTrackPro.Web.Models.Communication;
using HealthTrackPro.Web.Services.Implementations;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Areas.Trainer.Controllers
{
    /// <summary>Feedback the logged-in trainer gives to assigned clients.</summary>
    [Authorize(Roles = "Trainer")]
    public class FeedbackController : Controller
    {
        private readonly ITrainerService _trainerService;
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public FeedbackController()
        {
            _trainerService = new TrainerService();
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string trainerUserId = User.Identity.GetUserId();

            var items = await _db.TrainerFeedbacks
                .Where(f => f.TrainerUserId == trainerUserId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            var clients = await _trainerService.GetAssignedClientsAsync(trainerUserId);
            var names = clients.ToDictionary(c => c.UserId, c => c.FullName);

            var model = new FeedbackIndexViewModel();
            foreach (TrainerFeedback item in items)
            {
                model.Items.Add(new FeedbackRowViewModel
                {
                    ClientName = names.ContainsKey(item.ClientUserId) ? names[item.ClientUserId] : "Client no longer assigned",
                    RelatedArea = string.IsNullOrWhiteSpace(item.RelatedArea) ? "General" : item.RelatedArea,
                    FeedbackText = item.FeedbackText,
                    Rating = item.Rating.HasValue ? item.Rating.Value + " / 5" : "-",
                    NextFocus = item.NextFocus,
                    CreatedAt = item.CreatedAt.ToString("MMM d, yyyy h:mm tt")
                });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var model = new FeedbackCreateViewModel();
            await LoadClientsAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FeedbackCreateViewModel model)
        {
            string trainerUserId = User.Identity.GetUserId();

            if (ModelState.IsValid &&
                !await _trainerService.IsClientAssignedAsync(trainerUserId, model.ClientUserId))
            {
                ModelState.AddModelError("ClientUserId", "This client is not assigned to you.");
            }

            if (!ModelState.IsValid)
            {
                await LoadClientsAsync(model);
                return View(model);
            }

            _db.TrainerFeedbacks.Add(new TrainerFeedback
            {
                TrainerUserId = trainerUserId,
                ClientUserId = model.ClientUserId,
                RelatedArea = string.IsNullOrWhiteSpace(model.RelatedArea) ? null : model.RelatedArea.Trim(),
                FeedbackText = model.FeedbackText.Trim(),
                Rating = model.Rating,
                NextFocus = string.IsNullOrWhiteSpace(model.NextFocus) ? null : model.NextFocus.Trim(),
                CreatedAt = DateTime.Now
            });
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Feedback sent to your client.";
            return RedirectToAction("Index");
        }

        private async Task LoadClientsAsync(FeedbackCreateViewModel model)
        {
            var clients = await _trainerService.GetAssignedClientsAsync(User.Identity.GetUserId());
            model.Clients = clients
                .Select(c => new System.Web.Mvc.SelectListItem { Value = c.UserId, Text = c.FullName })
                .ToList();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                (_trainerService as IDisposable)?.Dispose();
                _db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
