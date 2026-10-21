using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using HealthTrackPro.Web.Enums;
using HealthTrackPro.Web.Models;
using HealthTrackPro.Web.Models.Scheduling;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Services.Implementations
{
    public class AppointmentService :
        IAppointmentService,
        IDisposable
    {
        private readonly ApplicationDbContext db;
        private readonly bool ownsContext;
        private readonly INotificationService notificationService;

        public AppointmentService()
            : this(new ApplicationDbContext(), true)
        {
        }

        public AppointmentService(
            ApplicationDbContext context)
            : this(context, false)
        {
        }

        private AppointmentService(
            ApplicationDbContext context,
            bool ownsContext)
        {
            if (context == null)
            {
                throw new ArgumentNullException(
                    "context");
            }

            db = context;
            this.ownsContext = ownsContext;

            notificationService =
                new NotificationService(db);
        }

        public async Task<List<Appointment>>
            GetClientAppointmentsAsync(
                string clientUserId)
        {
            if (string.IsNullOrWhiteSpace(
                clientUserId))
            {
                return new List<Appointment>();
            }

            return await db.Appointments
                .Include(a => a.Trainer)
                .Where(a =>
                    a.ClientUserId ==
                        clientUserId)
                .OrderByDescending(
                    a => a.AppointmentDateTime)
                .ToListAsync();
        }

        public async Task<List<Appointment>>
            GetTrainerAppointmentsAsync(
                string trainerUserId)
        {
            if (string.IsNullOrWhiteSpace(
                trainerUserId))
            {
                return new List<Appointment>();
            }

            return await db.Appointments
                .Include(a => a.Client)
                .Where(a =>
                    a.TrainerUserId ==
                        trainerUserId)
                .OrderByDescending(
                    a => a.AppointmentDateTime)
                .ToListAsync();
        }

        public async Task<Appointment>
            GetByIdAsync(
                int appointmentId)
        {
            return await db.Appointments
                .Include(a => a.Client)
                .Include(a => a.Trainer)
                .FirstOrDefaultAsync(
                    a => a.AppointmentId ==
                        appointmentId);
        }

        public async Task<bool> BookAsync(
            string clientUserId,
            string trainerUserId,
            DateTime appointmentDateTime,
            int durationMinutes,
            string reason,
            string notes)
        {
            if (string.IsNullOrWhiteSpace(
                    clientUserId) ||
                string.IsNullOrWhiteSpace(
                    trainerUserId))
            {
                return false;
            }

            var assigned =
                await db.TrainerClients
                    .AnyAsync(tc =>
                        tc.ClientUserId ==
                            clientUserId &&
                        tc.TrainerUserId ==
                            trainerUserId &&
                        tc.IsActive);

            if (!assigned)
            {
                return false;
            }

            var trainerApproved =
                await db.TrainerProfiles
                    .AnyAsync(t =>
                        t.UserId ==
                            trainerUserId &&
                        t.IsApproved);

            if (!trainerApproved)
            {
                return false;
            }

            var appointment =
                new Appointment
                {
                    ClientUserId =
                        clientUserId,

                    TrainerUserId =
                        trainerUserId,

                    Reason = reason,

                    Notes = notes,

                    CreatedAt =
                        DateTime.UtcNow
                };

            appointment.Schedule(
                appointmentDateTime,
                durationMinutes);

            db.Appointments.Add(
                appointment);

            await db.SaveChangesAsync();

            await CreateAppointmentNotificationAsync(
                trainerUserId,
                "New appointment request",
                "A client requested a " +
                durationMinutes +
                "-minute appointment for " +
                FormatDateTime(
                    appointmentDateTime) +
                "."
            );

            return true;
        }

        public async Task<bool> ApproveAsync(
            int appointmentId,
            string trainerUserId)
        {
            var appointment =
                await FindTrainerAppointmentAsync(
                    appointmentId,
                    trainerUserId);

            if (appointment == null)
            {
                return false;
            }

            appointment.Approve();

            await db.SaveChangesAsync();

            await CreateAppointmentNotificationAsync(
                appointment.ClientUserId,
                "Appointment approved",
                "Your trainer approved your appointment for " +
                FormatDateTime(
                    appointment.AppointmentDateTime) +
                "."
            );

            return true;
        }

        public async Task<bool> RejectAsync(
            int appointmentId,
            string trainerUserId)
        {
            var appointment =
                await FindTrainerAppointmentAsync(
                    appointmentId,
                    trainerUserId);

            if (appointment == null)
            {
                return false;
            }

            appointment.Reject();

            await db.SaveChangesAsync();

            await CreateAppointmentNotificationAsync(
                appointment.ClientUserId,
                "Appointment rejected",
                "Your appointment request for " +
                FormatDateTime(
                    appointment.AppointmentDateTime) +
                " was rejected by your trainer."
            );

            return true;
        }

        public async Task<bool> CancelAsync(
            int appointmentId,
            string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            var appointment =
                await db.Appointments
                    .FirstOrDefaultAsync(a =>
                        a.AppointmentId ==
                            appointmentId &&
                        (
                            a.ClientUserId ==
                                userId ||
                            a.TrainerUserId ==
                                userId
                        ));

            if (appointment == null)
            {
                return false;
            }

            string recipientUserId =
                appointment.ClientUserId == userId
                    ? appointment.TrainerUserId
                    : appointment.ClientUserId;

            appointment.Cancel();

            await db.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(
                recipientUserId))
            {
                await CreateAppointmentNotificationAsync(
                    recipientUserId,
                    "Appointment cancelled",
                    "The appointment scheduled for " +
                    FormatDateTime(
                        appointment.AppointmentDateTime) +
                    " was cancelled."
                );
            }

            return true;
        }

        public async Task<bool> RescheduleAsync(
            int appointmentId,
            string trainerUserId,
            DateTime newDateTime)
        {
            var appointment =
                await FindTrainerAppointmentAsync(
                    appointmentId,
                    trainerUserId);

            if (appointment == null)
            {
                return false;
            }

            appointment.Reschedule(
                newDateTime);

            await db.SaveChangesAsync();

            await CreateAppointmentNotificationAsync(
                appointment.ClientUserId,
                "Appointment rescheduled",
                "Your trainer rescheduled your appointment to " +
                FormatDateTime(
                    newDateTime) +
                "."
            );

            return true;
        }

        public async Task<bool> CompleteAsync(
            int appointmentId,
            string trainerUserId)
        {
            var appointment =
                await FindTrainerAppointmentAsync(
                    appointmentId,
                    trainerUserId);

            if (appointment == null)
            {
                return false;
            }

            appointment.Complete();

            await db.SaveChangesAsync();

            await CreateAppointmentNotificationAsync(
                appointment.ClientUserId,
                "Appointment completed",
                "Your appointment from " +
                FormatDateTime(
                    appointment.AppointmentDateTime) +
                " was marked as completed."
            );

            return true;
        }

        private Task<Appointment>
            FindTrainerAppointmentAsync(
                int appointmentId,
                string trainerUserId)
        {
            if (string.IsNullOrWhiteSpace(
                trainerUserId))
            {
                return Task.FromResult<
                    Appointment>(null);
            }

            return db.Appointments
                .FirstOrDefaultAsync(a =>
                    a.AppointmentId ==
                        appointmentId &&
                    a.TrainerUserId ==
                        trainerUserId);
        }

        private async Task
            CreateAppointmentNotificationAsync(
                string userId,
                string title,
                string message)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            await notificationService.CreateAsync(
                userId,
                title,
                message,
                NotificationType.AppointmentUpdate);
        }

        private static string FormatDateTime(
            DateTime value)
        {
            return value.ToString(
                "ddd, MMM d, yyyy 'at' h:mm tt");
        }

        public void Dispose()
        {
            if (ownsContext)
            {
                db.Dispose();
            }
        }
    }
}