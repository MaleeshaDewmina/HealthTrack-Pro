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

        //display the same statistics for an optional date range.
        [HttpGet]
        public async Task<ActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            ViewBag.ReportFrom = fromDate.HasValue
                ? fromDate.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.ReportTo = toDate.HasValue
                ? toDate.Value.ToString("yyyy-MM-dd") : "";

            bool invalidRange = fromDate.HasValue && toDate.HasValue &&
                fromDate.Value.Date > toDate.Value.Date;
            ViewBag.ReportError = invalidRange
                ? "The From date must not be later than the To date." : null;
            ViewBag.ReportFiltered = !invalidRange &&
                (fromDate.HasValue || toDate.HasValue);

            // If the input is invalid, display the original all-time report instead.
            var model = await BuildReportAsync(
                invalidRange ? null : fromDate,
                invalidRange ? null : toDate);
            return View(model);
        }

        // CSV is directly readable in Microsoft Excel, without a new NuGet package.
        [HttpGet]
        public async Task<ActionResult> ExportExcel(DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate.HasValue && toDate.HasValue &&
                fromDate.Value.Date > toDate.Value.Date)
            {
                return new HttpStatusCodeResult(400, "Invalid report date range.");
            }

            var model = await BuildReportAsync(fromDate, toDate);
            var rows = new[]
            {
                new [] { "HealthTrack Pro Reports & Analytics", "" },
                new [] { "From", fromDate.HasValue ? fromDate.Value.ToString("yyyy-MM-dd") : "All dates" },
                new [] { "To", toDate.HasValue ? toDate.Value.ToString("yyyy-MM-dd") : "All dates" },
                new [] { "Generated (UTC)", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm") },
                new [] { "Metric", "Value" },
                new [] { "Registered clients", model.TotalClients.ToString() },
                new [] { "Registered trainers", model.TotalTrainers.ToString() },
                new [] { "Suspended users (current snapshot)", model.SuspendedUsers.ToString() },
                new [] { "Active trainer assignments", model.ActiveAssignments.ToString() },
                new [] { "Unassigned clients in selection", model.UnassignedClients.ToString() },
                new [] { "Appointments", model.TotalAppointments.ToString() },
                new [] { "Pending appointments", model.PendingAppointments.ToString() },
                new [] { "Approved appointments", model.ApprovedAppointments.ToString() },
                new [] { "Rejected appointments", model.RejectedAppointments.ToString() },
                new [] { "Cancelled appointments", model.CancelledAppointments.ToString() },
                new [] { "Completed appointments", model.CompletedAppointments.ToString() },
                new [] { "Workout plans", model.TotalWorkoutPlans.ToString() },
                new [] { "Active workout plans", model.ActiveWorkoutPlans.ToString() },
                new [] { "Workout logs", model.WorkoutLogs.ToString() },
                new [] { "Diet plans", model.TotalDietPlans.ToString() },
                new [] { "Active diet plans", model.ActiveDietPlans.ToString() },
                new [] { "Meal logs", model.MealLogs.ToString() },
                new [] { "Fitness goals", model.TotalGoals.ToString() },
                new [] { "Completed fitness goals", model.CompletedGoals.ToString() },
                new [] { "Health measurements", model.HealthMeasurements.ToString() },
                new [] { "Notifications", model.Notifications.ToString() },
                new [] { "Unread notifications", model.UnreadNotifications.ToString() },
                new [] { "Goal completion (%)", model.GoalCompletionPercentage.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) },
                new [] { "Trainer assignment coverage (%)", model.AssignmentCoveragePercentage.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) },
                new [] { "Appointment completion (%)", model.AppointmentCompletionPercentage.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) },
                new [] { "Date basis", "Profile creation / plan start / activity logged / appointment scheduled" }
            };

            // Excel recognises the sep= directive and UTF-8 BOM for CSV data.
            var sb = new System.Text.StringBuilder("\uFEFFsep=,\r\n");
            foreach (var row in rows)
            {
                sb.AppendLine(string.Join(",", row.Select(CsvCell)));
            }

            var fileName = "HealthTrack_Report_" +
                DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
            return File(System.Text.Encoding.UTF8.GetBytes(sb.ToString()),
                "text/csv; charset=utf-8", fileName);
        }

        private static string CsvCell(string value)
        {
            return "\"" + (value ?? "").Replace("\"", "\"\"") + "\"";
        }

        private async Task<AdminReportsViewModel> BuildReportAsync(
            DateTime? fromDate, DateTime? toDate)
        {
            DateTime? start = fromDate.HasValue ? fromDate.Value.Date : (DateTime?)null;
            DateTime? end = toDate.HasValue ? toDate.Value.Date.AddDays(1) : (DateTime?)null;
            var now = DateTime.UtcNow;

            var clients = db.ClientProfiles.AsQueryable();
            var trainers = db.TrainerProfiles.AsQueryable();
            var assignments = db.TrainerClients.AsQueryable();
            var appointments = db.Appointments.AsQueryable();
            var workoutPlans = db.WorkoutPlans.AsQueryable();
            var workoutLogs = db.WorkoutLogs.AsQueryable();
            var dietPlans = db.DietPlans.AsQueryable();
            var mealLogs = db.MealLogs.AsQueryable();
            var goals = db.FitnessGoals.AsQueryable();
            var measurements = db.HealthMetrics.AsQueryable();
            var notifications = db.Notifications.AsQueryable();

            if (start.HasValue)
            {
                clients = clients.Where(x => x.CreatedAt >= start.Value);
                trainers = trainers.Where(x => x.CreatedAt >= start.Value);
                assignments = assignments.Where(x => x.AssignedAt >= start.Value);
                appointments = appointments.Where(x => x.AppointmentDateTime >= start.Value);
                workoutPlans = workoutPlans.Where(x => x.StartDate >= start.Value);
                workoutLogs = workoutLogs.Where(x => x.StartedAt >= start.Value);
                dietPlans = dietPlans.Where(x => x.StartDate >= start.Value);
                mealLogs = mealLogs.Where(x => x.LoggedAt >= start.Value);
                goals = goals.Where(x => x.StartDate >= start.Value);
                measurements = measurements.Where(x => x.RecordedAt >= start.Value);
                notifications = notifications.Where(x => x.CreatedAt >= start.Value);
            }
            if (end.HasValue)
            {
                clients = clients.Where(x => x.CreatedAt < end.Value);
                trainers = trainers.Where(x => x.CreatedAt < end.Value);
                assignments = assignments.Where(x => x.AssignedAt < end.Value);
                appointments = appointments.Where(x => x.AppointmentDateTime < end.Value);
                workoutPlans = workoutPlans.Where(x => x.StartDate < end.Value);
                workoutLogs = workoutLogs.Where(x => x.StartedAt < end.Value);
                dietPlans = dietPlans.Where(x => x.StartDate < end.Value);
                mealLogs = mealLogs.Where(x => x.LoggedAt < end.Value);
                goals = goals.Where(x => x.StartDate < end.Value);
                measurements = measurements.Where(x => x.RecordedAt < end.Value);
                notifications = notifications.Where(x => x.CreatedAt < end.Value);
            }

            var totalClients = await clients.CountAsync();
            var assignedClientCount = await assignments
                .Where(a => a.IsActive)
                .Select(a => a.ClientUserId)
                .Distinct()
                .CountAsync();

            return new AdminReportsViewModel
            {
                TotalClients = totalClients,
                TotalTrainers = await trainers.CountAsync(),
                // LockoutEndDateUtc is a live account state, not a dated event.
                SuspendedUsers = await db.Users.CountAsync(u =>
                    u.LockoutEndDateUtc.HasValue && u.LockoutEndDateUtc.Value > now),
                ActiveAssignments = await assignments.CountAsync(x => x.IsActive),
                UnassignedClients = Math.Max(0, totalClients - assignedClientCount),
                TotalAppointments = await appointments.CountAsync(),
                PendingAppointments = await appointments.CountAsync(x => x.Status == AppointmentStatus.Pending),
                ApprovedAppointments = await appointments.CountAsync(x => x.Status == AppointmentStatus.Approved),
                RejectedAppointments = await appointments.CountAsync(x => x.Status == AppointmentStatus.Rejected),
                CancelledAppointments = await appointments.CountAsync(x => x.Status == AppointmentStatus.Cancelled),
                CompletedAppointments = await appointments.CountAsync(x => x.Status == AppointmentStatus.Completed),
                TotalWorkoutPlans = await workoutPlans.CountAsync(),
                ActiveWorkoutPlans = await workoutPlans.CountAsync(x => x.IsActive),
                WorkoutLogs = await workoutLogs.CountAsync(),
                TotalDietPlans = await dietPlans.CountAsync(),
                ActiveDietPlans = await dietPlans.CountAsync(x => x.IsActive),
                MealLogs = await mealLogs.CountAsync(),
                TotalGoals = await goals.CountAsync(),
                CompletedGoals = await goals.CountAsync(x => x.IsCompleted),
                HealthMeasurements = await measurements.CountAsync(),
                Notifications = await notifications.CountAsync(),
                UnreadNotifications = await notifications.CountAsync(x => !x.IsRead)
            };
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
