using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using HealthTrackPro.Web.Areas.Trainer.Models;
using HealthTrackPro.Web.Enums;
using HealthTrackPro.Web.Models.Scheduling;
using HealthTrackPro.Web.Services.Implementations;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Areas.Trainer.Controllers
{
    /// <summary>Appointments between the logged-in trainer and their assigned clients.</summary>
    [Authorize(Roles = "Trainer")]
    public class AppointmentsController : Controller
    {
        private readonly ITrainerService _trainerService;
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController()
        {
            _trainerService = new TrainerService();
            _appointmentService = new AppointmentService();
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string trainerUserId = User.Identity.GetUserId();

            var appointments = await _appointmentService.GetTrainerAppointmentsAsync(trainerUserId);
            var clients = await _trainerService.GetAssignedClientsAsync(trainerUserId);
            var names = clients.ToDictionary(c => c.UserId, c => c.FullName);

            var model = new AppointmentIndexViewModel();
            foreach (Appointment appointment in appointments.OrderBy(x => x.AppointmentDateTime))
            {
                model.Appointments.Add(new AppointmentRowViewModel
                {
                    AppointmentId = appointment.AppointmentId,
                    ClientName = names.ContainsKey(appointment.ClientUserId)
                        ? names[appointment.ClientUserId]
                        : "Client no longer assigned",
                    When = appointment.AppointmentDateTime.ToString("ddd, MMM d yyyy 'at' h:mm tt"),
                    DurationMinutes = appointment.DurationMinutes,
                    Reason = appointment.Reason,
                    Status = appointment.Status.ToString(),
                    IsPending = appointment.Status == AppointmentStatus.Pending,
                    IsApproved = appointment.Status == AppointmentStatus.Approved,
                    CanReschedule = appointment.Status == AppointmentStatus.Pending
                        || appointment.Status == AppointmentStatus.Approved
                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Approve(int id)
        {
            bool ok = await _appointmentService.ApproveAsync(id, User.Identity.GetUserId());
            return Done(ok, "Appointment approved.", "That appointment could not be approved.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Reject(int id)
        {
            bool ok = await _appointmentService.RejectAsync(id, User.Identity.GetUserId());
            return Done(ok, "Appointment rejected.", "That appointment could not be rejected.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Complete(int id)
        {
            bool ok = await _appointmentService.CompleteAsync(id, User.Identity.GetUserId());
            return Done(ok, "Appointment marked as completed.", "Only approved appointments can be completed.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Reschedule(int id, DateTime newDateTime)
        {
            if (newDateTime <= DateTime.Now)
            {
                TempData["ErrorMessage"] = "Choose a date and time in the future.";
                return RedirectToAction("Index");
            }

            bool ok = await _appointmentService.RescheduleAsync(id, User.Identity.GetUserId(), newDateTime);
            return Done(ok,
                "Appointment moved to the new time. It is pending approval again.",
                "That appointment could not be rescheduled.");
        }

        private ActionResult Done(bool ok, string successMessage, string errorMessage)
        {
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok ? successMessage : errorMessage;
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                (_trainerService as IDisposable)?.Dispose();
                (_appointmentService as IDisposable)?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
