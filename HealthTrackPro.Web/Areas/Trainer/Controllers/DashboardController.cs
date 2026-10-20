using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using HealthTrackPro.Web.Areas.Trainer.Models;
using HealthTrackPro.Web.Enums;
using HealthTrackPro.Web.Models.Communication;
using HealthTrackPro.Web.Models.Profiles;
using HealthTrackPro.Web.Models.Scheduling;
using HealthTrackPro.Web.Services.Implementations;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Areas.Trainer.Controllers
{
    [Authorize(Roles = "Trainer")]
    public class DashboardController : Controller
    {
        private readonly ITrainerService _trainerService;
        private readonly IAppointmentService _appointmentService;
        private readonly IWorkoutService _workoutService;
        private readonly IDietService _dietService;
        private readonly INotificationService _notificationService;

        public DashboardController()
        {
            _trainerService = new TrainerService();
            _appointmentService = new AppointmentService();
            _workoutService = new WorkoutService();
            _dietService = new DietService();
            _notificationService = new NotificationService();
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            // Everything below is scoped to the signed-in trainer's own id.
            string trainerUserId = User.Identity.GetUserId();

            List<ClientProfile> clients =
                await _trainerService.GetAssignedClientsAsync(trainerUserId);

            List<Appointment> appointments =
                await _appointmentService.GetTrainerAppointmentsAsync(trainerUserId);

            var workoutPlans = await _workoutService.GetTrainerPlansAsync(trainerUserId);
            var dietPlans = await _dietService.GetTrainerPlansAsync(trainerUserId);
            int unreadCount = await _notificationService.GetUnreadCountAsync(trainerUserId);
            var notifications =
                await _notificationService.GetUnreadNotificationsAsync(trainerUserId);

            DateTime now = DateTime.Now;

            var model = new TrainerDashboardViewModel
            {
                AssignedClientCount = clients.Count,
                PendingAppointmentCount = appointments.Count(a => a.Status == AppointmentStatus.Pending),
                ActiveWorkoutPlanCount = workoutPlans.Count(p => p.IsActive),
                ActiveDietPlanCount = dietPlans.Count(p => p.IsActive),
                UnreadNotificationCount = unreadCount
            };

            foreach (ClientProfile client in clients.Take(5))
            {
                model.RecentClients.Add(new DashboardClientRowViewModel
                {
                    ClientUserId = client.UserId,
                    FullName = string.IsNullOrWhiteSpace(client.FullName) ? "Unnamed client" : client.FullName,
                    PrimaryGoal = client.PrimaryGoal.HasValue
                        ? System.Text.RegularExpressions.Regex.Replace(client.PrimaryGoal.Value.ToString(), "(?<!^)([A-Z])", " $1")
                        : "Not set"
                });
            }

            // Upcoming = pending or approved, and not in the past.
            foreach (Appointment appointment in appointments
                .Where(a => (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Approved)
                            && a.AppointmentDateTime >= now)
                .OrderBy(a => a.AppointmentDateTime)
                .Take(5))
            {
                model.UpcomingAppointments.Add(new DashboardAppointmentRowViewModel
                {
                    ClientName = appointment.Client != null ? appointment.Client.FullName : "Client",
                    When = appointment.AppointmentDateTime.ToString("ddd, MMM d - h:mm tt"),
                    Status = appointment.Status.ToString()
                });
            }

            foreach (Notification notification in notifications.Take(5))
            {
                model.RecentNotifications.Add(new DashboardNotificationRowViewModel
                {
                    Title = notification.Title,
                    Message = notification.Message,
                    When = notification.CreatedAt.ToString("MMM d, h:mm tt"),
                    IsRead = notification.IsRead
                });
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                (_trainerService as IDisposable)?.Dispose();
                (_appointmentService as IDisposable)?.Dispose();
                (_workoutService as IDisposable)?.Dispose();
                (_dietService as IDisposable)?.Dispose();
                (_notificationService as IDisposable)?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
