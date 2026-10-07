using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthTrackPro.Web.Enums;
using HealthTrackPro.Web.Models.Profiles;

namespace HealthTrackPro.Web.Models.Scheduling
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("Client")]
        public string ClientUserId { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("Trainer")]
        public string TrainerUserId { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; private set; }

        [Range(15, 240)]
        public int DurationMinutes { get; private set; }

        [Required]
        [StringLength(200)]
        public string Reason { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        public AppointmentStatus Status { get; private set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; private set; }

        public virtual ClientProfile Client { get; set; }

        public virtual TrainerProfile Trainer { get; set; }

        public void Schedule(DateTime appointmentDateTime, int durationMinutes)
        {
            if (appointmentDateTime <= DateTime.Now)
            {
                throw new InvalidOperationException(
                    "Appointment must be scheduled for a future date and time.");
            }

            if (durationMinutes < 15 || durationMinutes > 240)
            {
                throw new ArgumentOutOfRangeException(
                    "durationMinutes",
                    "Appointment duration must be between 15 and 240 minutes.");
            }

            AppointmentDateTime = appointmentDateTime;
            DurationMinutes = durationMinutes;
            Status = AppointmentStatus.Pending;
            UpdatedAt = DateTime.Now;
        }

        public void Approve()
        {
            if (Status != AppointmentStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending appointments can be approved.");
            }

            Status = AppointmentStatus.Approved;
            UpdatedAt = DateTime.Now;
        }

        public void Reject()
        {
            if (Status != AppointmentStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending appointments can be rejected.");
            }

            Status = AppointmentStatus.Rejected;
            UpdatedAt = DateTime.Now;
        }

        public void Cancel()
        {
            if (Status == AppointmentStatus.Completed)
            {
                throw new InvalidOperationException(
                    "Completed appointments cannot be cancelled.");
            }

            Status = AppointmentStatus.Cancelled;
            UpdatedAt = DateTime.Now;
        }

        public void Reschedule(DateTime newDateTime)
        {
            if (newDateTime <= DateTime.Now)
            {
                throw new InvalidOperationException(
                    "New appointment time must be in the future.");
            }

            if (Status == AppointmentStatus.Completed ||
                Status == AppointmentStatus.Cancelled)
            {
                throw new InvalidOperationException(
                    "This appointment cannot be rescheduled.");
            }

            AppointmentDateTime = newDateTime;
            Status = AppointmentStatus.Pending;
            UpdatedAt = DateTime.Now;
        }

        public void Complete()
        {
            if (Status != AppointmentStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Only approved appointments can be completed.");
            }

            Status = AppointmentStatus.Completed;
            UpdatedAt = DateTime.Now;
        }
    }
}
