
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;

using HealthTrackPro.Web.Areas.Client.Models;
using HealthTrackPro.Web.Enums;
using HealthTrackPro.Web.Models;
using HealthTrackPro.Web.Models.Profiles;
using HealthTrackPro.Web.Services.Implementations;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Areas.Client.Controllers
{
    [Authorize(Roles = "Client")]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService appointmentService;
        private readonly ApplicationDbContext db;

        public AppointmentsController()
        {
            appointmentService = new AppointmentService();
            db = new ApplicationDbContext();
        }

        // GET: /Client/Appointments
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var model = await LoadPageAsync(
                new ClientAppointmentBookingInputViewModel());

            return View(model);
        }

        // POST: /Client/Appointments/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Book(
            [Bind(Prefix = "NewBooking")]
            ClientAppointmentBookingInputViewModel input)
        {
            if (input == null)
            {
                input = new ClientAppointmentBookingInputViewModel();

                ModelState.AddModelError(
                    "",
                    "Please enter your appointment details.");
            }

            if (input.AppointmentDateTime.HasValue &&
                input.AppointmentDateTime.Value <= DateTime.Now)
            {
                ModelState.AddModelError(
                    "NewBooking.AppointmentDateTime",
                    "Please select a future appointment date and time.");
            }

            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return new HttpUnauthorizedResult();
                }

                bool trainerIsEligible =
                    await db.TrainerClients.AnyAsync(tc =>
                        tc.ClientUserId == userId &&
                        tc.TrainerUserId == input.TrainerUserId &&
                        tc.IsActive &&
                        tc.Trainer.IsApproved);

                if (!trainerIsEligible)
                {
                    ModelState.AddModelError(
                        "NewBooking.TrainerUserId",
                        "Please select an active, approved trainer assigned to you.");
                }
                else
                {
                    try
                    {
                        bool booked = await appointmentService.BookAsync(
                            userId,
                            input.TrainerUserId,
                            input.AppointmentDateTime.Value,
                            input.DurationMinutes.Value,
                            input.Reason.Trim(),
                            string.IsNullOrWhiteSpace(input.Notes)
                                ? null
                                : input.Notes.Trim());

                        if (booked)
                        {
                            TempData["AppointmentSuccess"] =
                                "Your appointment request was submitted successfully. " +
                                "It is awaiting trainer approval.";

                            return RedirectToAction("Index");
                        }

                        ModelState.AddModelError(
                            "",
                            "The appointment could not be booked. " +
                            "Please verify your trainer assignment and try again.");
                    }
                    catch (InvalidOperationException ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                    }
                }
            }

            var model = await LoadPageAsync(input);

            return View("Index", model);
        }

        // POST: /Client/Appointments/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Cancel(int id)
        {
            string userId = User.Identity.GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new HttpUnauthorizedResult();
            }

            // The service also checks ownership, but explicitly
            // verify it here before allowing cancellation.
            var appointment = await db.Appointments
                .AsNoTracking()
                .FirstOrDefaultAsync(a =>
                    a.AppointmentId == id &&
                    a.ClientUserId == userId);

            if (appointment == null)
            {
                return HttpNotFound();
            }

            if (appointment.Status == AppointmentStatus.Completed ||
                appointment.Status == AppointmentStatus.Cancelled ||
                appointment.Status == AppointmentStatus.Rejected)
            {
                TempData["AppointmentError"] =
                    "This appointment cannot be cancelled.";

                return RedirectToAction("Index");
            }

            try
            {
                bool cancelled =
                    await appointmentService.CancelAsync(id, userId);

                TempData[cancelled
                    ? "AppointmentSuccess"
                    : "AppointmentError"] =
                    cancelled
                        ? "Your appointment was cancelled successfully."
                        : "Unable to cancel the appointment.";

            }
            catch (InvalidOperationException ex)
            {
                TempData["AppointmentError"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        private async Task<ClientAppointmentsViewModel> LoadPageAsync(
            ClientAppointmentBookingInputViewModel input)
        {
            var model = new ClientAppointmentsViewModel
            {
                NewBooking = input ??
                    new ClientAppointmentBookingInputViewModel()
            };

            string userId = User.Identity.GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return model;
            }

            model.Appointments = await appointmentService
                .GetClientAppointmentsAsync(userId);

            var assignedTrainers = await db.TrainerClients
                .AsNoTracking()
                .Include(tc => tc.Trainer)
                .Where(tc =>
                    tc.ClientUserId == userId &&
                    tc.IsActive &&
                    tc.Trainer.IsApproved)
                .ToListAsync();

            model.AvailableTrainers = assignedTrainers
                .Where(tc => tc.Trainer != null)
                .GroupBy(tc => tc.TrainerUserId)
                .Select(group =>
                {
                    TrainerProfile trainer = group.First().Trainer;

                    return new ClientAppointmentTrainerViewModel
                    {
                        TrainerUserId = trainer.UserId,
                        FullName = trainer.FullName,
                        Specialization = trainer.Specialization
                    };
                })
                .OrderBy(t => t.FullName)
                .ToList();

            return model;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposable = appointmentService as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }

                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
