using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using HealthTrackPro.Web.Areas.Admin.Models;
using HealthTrackPro.Web.Enums;
using HealthTrackPro.Web.Models;

namespace HealthTrackPro.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext db =
            new ApplicationDbContext();

        public async Task<ActionResult> Index()
        {
            var now = DateTime.UtcNow;

            var totalClients =
                await db.ClientProfiles.CountAsync();

            var totalTrainers =
                await db.TrainerProfiles.CountAsync();

            var suspendedUsers =
                await db.Users.CountAsync(u =>
                    u.LockoutEndDateUtc.HasValue &&
                    u.LockoutEndDateUtc.Value > now);

            var activeAssignments =
                await db.TrainerClients.CountAsync(a =>
                    a.IsActive);

            var assignedClientIds =
                await db.TrainerClients
                    .Where(a => a.IsActive)
                    .Select(a => a.ClientUserId)
                    .Distinct()
                    .CountAsync();

            var model =
                new AdminReportsViewModel
                {
                    TotalClients =
                        totalClients,

                    TotalTrainers =
                        totalTrainers,

                    SuspendedUsers =
                        suspendedUsers,

                    ActiveAssignments =
                        activeAssignments,

                    UnassignedClients =
                        Math.Max(
                            0,
                            totalClients -
                            assignedClientIds),

                    TotalAppointments =
                        await db.Appointments.CountAsync(),

                    PendingAppointments =
                        await db.Appointments.CountAsync(a =>
                            a.Status ==
                            AppointmentStatus.Pending),

                    ApprovedAppointments =
                        await db.Appointments.CountAsync(a =>
                            a.Status ==
                            AppointmentStatus.Approved),

                    RejectedAppointments =
                        await db.Appointments.CountAsync(a =>
                            a.Status ==
                            AppointmentStatus.Rejected),

                    CancelledAppointments =
                        await db.Appointments.CountAsync(a =>
                            a.Status ==
                            AppointmentStatus.Cancelled),

                    CompletedAppointments =
                        await db.Appointments.CountAsync(a =>
                            a.Status ==
                            AppointmentStatus.Completed),

                    TotalWorkoutPlans =
                        await db.WorkoutPlans.CountAsync(),

                    ActiveWorkoutPlans =
                        await db.WorkoutPlans.CountAsync(w =>
                            w.IsActive),

                    WorkoutLogs =
                        await db.WorkoutLogs.CountAsync(),

                    TotalDietPlans =
                        await db.DietPlans.CountAsync(),

                    ActiveDietPlans =
                        await db.DietPlans.CountAsync(d =>
                            d.IsActive),

                    MealLogs =
                        await db.MealLogs.CountAsync(),

                    TotalGoals =
                        await db.FitnessGoals.CountAsync(),

                    CompletedGoals =
                        await db.FitnessGoals.CountAsync(g =>
                            g.IsCompleted),

                    HealthMeasurements =
                        await db.HealthMetrics.CountAsync(),

                    Notifications =
                        await db.Notifications.CountAsync(),

                    UnreadNotifications =
                        await db.Notifications.CountAsync(n =>
                            !n.IsRead)
                };

            return View(model);
        }

        protected override void Dispose(
            bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
