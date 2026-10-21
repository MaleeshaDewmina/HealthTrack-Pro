using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using HealthTrackPro.Web.Areas.Trainer.Models;
using HealthTrackPro.Web.Models;
using HealthTrackPro.Web.Models.Scheduling;

namespace HealthTrackPro.Web.Areas.Trainer.Controllers
{
    /// <summary>The logged-in trainer's own availability slots.</summary>
    [Authorize(Roles = "Trainer")]
    public class AvailabilityController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return View(await BuildIndexAsync(new AvailabilityCreateViewModel()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add([Bind(Prefix = "Form")] AvailabilityCreateViewModel form)
        {
            TimeSpan start = TimeSpan.Zero;
            TimeSpan end = TimeSpan.Zero;

            if (!TimeSpan.TryParse(form.StartTime, out start))
            {
                ModelState.AddModelError("StartTime", "Enter a valid start time.");
            }

            if (!TimeSpan.TryParse(form.EndTime, out end))
            {
                ModelState.AddModelError("EndTime", "Enter a valid end time.");
            }

            if (ModelState.IsValid && end <= start)
            {
                ModelState.AddModelError("EndTime", "End time must be later than start time.");
            }

            if (form.Date.Date < DateTime.Today)
            {
                ModelState.AddModelError("Date", "Choose today or a future date.");
            }

            if (!ModelState.IsValid)
            {
                return View("Index", await BuildIndexAsync(form));
            }

            var slot = new TrainerAvailability
            {
                TrainerUserId = User.Identity.GetUserId(),
                AvailableDate = form.Date.Date,
                StartTime = start,
                EndTime = end,
                Notes = form.Notes
            };
            slot.OpenSlot();

            _db.TrainerAvailabilities.Add(slot);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Availability slot added.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Toggle(int id, bool open)
        {
            string trainerUserId = User.Identity.GetUserId();

            // Only the trainer's own slots are found, so other trainers' slots cannot be changed.
            var slot = await _db.TrainerAvailabilities
                .FirstOrDefaultAsync(a => a.TrainerAvailabilityId == id && a.TrainerUserId == trainerUserId);

            if (slot == null)
            {
                TempData["ErrorMessage"] = "That slot could not be found.";
                return RedirectToAction("Index");
            }

            try
            {
                if (open)
                {
                    slot.OpenSlot();
                }
                else
                {
                    slot.CloseSlot();
                }

                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = open ? "Slot opened." : "Slot closed.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        private async Task<AvailabilityIndexViewModel> BuildIndexAsync(AvailabilityCreateViewModel form)
        {
            string trainerUserId = User.Identity.GetUserId();
            DateTime today = DateTime.Today;

            var slots = await _db.TrainerAvailabilities
                .Where(a => a.TrainerUserId == trainerUserId && a.AvailableDate >= today)
                .OrderBy(a => a.AvailableDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();

            var model = new AvailabilityIndexViewModel { Form = form };
            foreach (var slot in slots)
            {
                model.Slots.Add(new AvailabilitySlotRowViewModel
                {
                    SlotId = slot.TrainerAvailabilityId,
                    Date = slot.AvailableDate.ToString("ddd, MMM d yyyy"),
                    Time = FormatTime(slot.StartTime) + " to " + FormatTime(slot.EndTime),
                    Notes = slot.Notes,
                    IsOpen = slot.IsAvailable
                });
            }

            return model;
        }

        private static string FormatTime(TimeSpan time)
        {
            return DateTime.Today.Add(time).ToString("h:mm tt");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
