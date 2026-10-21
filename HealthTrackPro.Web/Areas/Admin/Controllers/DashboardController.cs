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
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext db =
            new ApplicationDbContext();

        public async Task<ActionResult> Index()
        {
            var now = DateTime.UtcNow;

            var recentLogs =
                await db.AuditLogs
                    .AsNoTracking()
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(6)
                    .ToListAsync();

            var userIds =
                recentLogs
                    .Where(a =>
                        !string.IsNullOrWhiteSpace(a.UserId))
                    .Select(a => a.UserId)
                    .Distinct()
                    .ToList();

            var users =
                await db.Users
                    .AsNoTracking()
                    .Where(u =>
                        userIds.Contains(u.Id))
                    .Select(u => new
                    {
                        u.Id,
                        u.Email
                    })
                    .ToListAsync();

            var userEmails =
                users.ToDictionary(
                    u => u.Id,
                    u => u.Email);

            var recentAppointments =
                await db.Appointments
                    .AsNoTracking()
                    .Include(a => a.Client)
                    .Include(a => a.Trainer)
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(5)
                    .ToListAsync();

            var model =
                new AdminDashboardViewModel
                {
                    TotalClients =
                        await db.ClientProfiles.CountAsync(),

                    TotalTrainers =
                        await db.TrainerProfiles.CountAsync(),

                    ActiveAssignments =
                        await db.TrainerClients.CountAsync(a =>
                            a.IsActive),

                    PendingAppointments =
                        await db.Appointments.CountAsync(a =>
                            a.Status ==
                            AppointmentStatus.Pending),

                    ApprovedAppointments =
                        await db.Appointments.CountAsync(a =>
                            a.Status ==
                            AppointmentStatus.Approved),

                    CompletedAppointments =
                        await db.Appointments.CountAsync(a =>
                            a.Status ==
                            AppointmentStatus.Completed),

                    CancelledAppointments =
                        await db.Appointments.CountAsync(a =>
                            a.Status ==
                            AppointmentStatus.Cancelled),

                    RejectedAppointments =
                        await db.Appointments.CountAsync(a =>
                            a.Status ==
                            AppointmentStatus.Rejected),

                    SuspendedUsers =
                        await db.Users.CountAsync(u =>
                            u.LockoutEndDateUtc.HasValue &&
                            u.LockoutEndDateUtc.Value > now),

                    CompletedGoals =
                        await db.FitnessGoals.CountAsync(g =>
                            g.IsCompleted),

                    ActiveWorkoutPlans =
                        await db.WorkoutPlans.CountAsync(w =>
                            w.IsActive),

                    ActiveDietPlans =
                        await db.DietPlans.CountAsync(d =>
                            d.IsActive),

                    UnreadNotifications =
                        await db.Notifications.CountAsync(n =>
                            !n.IsRead),

                    RecentActivities =
                        recentLogs
                            .Select(log =>
                                new AdminDashboardActivityViewModel
                                {
                                    UserEmail =
                                        !string.IsNullOrWhiteSpace(
                                            log.UserId) &&
                                        userEmails.ContainsKey(
                                            log.UserId)
                                            ? userEmails[log.UserId]
                                            : "System",

                                    Action =
                                        log.Action,

                                    Description =
                                        log.Description,

                                    CreatedAt =
                                        log.CreatedAt
                                })
                            .ToList(),

                    RecentAppointments =
                        recentAppointments
                            .Select(a =>
                                new AdminDashboardAppointmentViewModel
                                {
                                    AppointmentId =
                                        a.AppointmentId,

                                    ClientName =
                                        a.Client != null
                                            ? a.Client.FullName
                                            : "Unknown Client",

                                    TrainerName =
                                        a.Trainer != null
                                            ? a.Trainer.FullName
                                            : "Unknown Trainer",

                                    AppointmentDateTime =
                                        a.AppointmentDateTime,

                                    Status =
                                        a.Status
                                })
                            .ToList()
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
