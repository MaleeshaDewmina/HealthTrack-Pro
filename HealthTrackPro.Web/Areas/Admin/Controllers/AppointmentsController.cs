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
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext db =
            new ApplicationDbContext();

        public async Task<ActionResult> Index(
            string search,
            string status)
        {
            var appointments =
                await db.Appointments
                    .AsNoTracking()
                    .Include(a => a.Client)
                    .Include(a => a.Trainer)
                    .OrderByDescending(
                        a => a.AppointmentDateTime)
                    .ToListAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalizedSearch =
                    search.Trim();

                appointments =
                    appointments
                        .Where(a =>
                            (
                                a.Client != null &&
                                a.Client.FullName != null &&
                                a.Client.FullName.IndexOf(
                                    normalizedSearch,
                                    StringComparison
                                        .OrdinalIgnoreCase) >= 0
                            )
                            ||
                            (
                                a.Trainer != null &&
                                a.Trainer.FullName != null &&
                                a.Trainer.FullName.IndexOf(
                                    normalizedSearch,
                                    StringComparison
                                        .OrdinalIgnoreCase) >= 0
                            )
                            ||
                            (
                                a.Reason != null &&
                                a.Reason.IndexOf(
                                    normalizedSearch,
                                    StringComparison
                                        .OrdinalIgnoreCase) >= 0
                            ))
                        .ToList();
            }

            AppointmentStatus parsedStatus;

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse(
                    status,
                    true,
                    out parsedStatus))
            {
                appointments =
                    appointments
                        .Where(a =>
                            a.Status ==
                                parsedStatus)
                        .ToList();
            }

            var model =
                new AdminAppointmentsIndexViewModel
                {
                    SearchTerm = search,
                    StatusFilter = status,
                    Appointments =
                        appointments
                            .Select(a =>
                                new AdminAppointmentListItemViewModel
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

                                    DurationMinutes =
                                        a.DurationMinutes,

                                    Reason =
                                        a.Reason,

                                    Notes =
                                        a.Notes,

                                    Status =
                                        a.Status,

                                    CreatedAt =
                                        a.CreatedAt,

                                    UpdatedAt =
                                        a.UpdatedAt
                                })
                            .ToList()
                };

            return View(model);
        }

        public async Task<ActionResult> Details(
            int id)
        {
            var appointment =
                await db.Appointments
                    .AsNoTracking()
                    .Include(a => a.Client)
                    .Include(a => a.Trainer)
                    .FirstOrDefaultAsync(a =>
                        a.AppointmentId == id);

            if (appointment == null)
            {
                return HttpNotFound();
            }

            var model =
                new AdminAppointmentListItemViewModel
                {
                    AppointmentId =
                        appointment.AppointmentId,

                    ClientName =
                        appointment.Client != null
                            ? appointment.Client.FullName
                            : "Unknown Client",

                    TrainerName =
                        appointment.Trainer != null
                            ? appointment.Trainer.FullName
                            : "Unknown Trainer",

                    AppointmentDateTime =
                        appointment.AppointmentDateTime,

                    DurationMinutes =
                        appointment.DurationMinutes,

                    Reason =
                        appointment.Reason,

                    Notes =
                        appointment.Notes,

                    Status =
                        appointment.Status,

                    CreatedAt =
                        appointment.CreatedAt,

                    UpdatedAt =
                        appointment.UpdatedAt
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
