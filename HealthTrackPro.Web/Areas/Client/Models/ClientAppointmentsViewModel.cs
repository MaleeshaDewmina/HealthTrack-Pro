
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using HealthTrackPro.Web.Models.Scheduling;

namespace HealthTrackPro.Web.Areas.Client.Models
{
    public class ClientAppointmentsViewModel
    {
        public List<Appointment> Appointments
        {
            get;
            set;
        }

        public List<ClientAppointmentTrainerViewModel> AvailableTrainers
        {
            get;
            set;
        }

        public ClientAppointmentBookingInputViewModel NewBooking
        {
            get;
            set;
        }

        public ClientAppointmentsViewModel()
        {
            Appointments = new List<Appointment>();

            AvailableTrainers =
                new List<ClientAppointmentTrainerViewModel>();

            NewBooking =
                new ClientAppointmentBookingInputViewModel();
        }
    }

    public class ClientAppointmentTrainerViewModel
    {
        public string TrainerUserId
        {
            get;
            set;
        }

        public string FullName
        {
            get;
            set;
        }

        public string Specialization
        {
            get;
            set;
        }
    }

    public class ClientAppointmentBookingInputViewModel
    {
        [Required(
            ErrorMessage = "Please select your assigned trainer.")]
        [Display(Name = "Trainer")]
        public string TrainerUserId
        {
            get;
            set;
        }

        [Required(
            ErrorMessage = "Please select an appointment date and time.")]
        [Display(Name = "Appointment Date and Time")]
        public DateTime? AppointmentDateTime
        {
            get;
            set;
        }

        [Required(
            ErrorMessage = "Please select the appointment duration.")]
        [Range(
            15,
            240,
            ErrorMessage =
                "Duration must be between 15 and 240 minutes.")]
        [Display(Name = "Duration (minutes)")]
        public int? DurationMinutes
        {
            get;
            set;
        }

        [Required(
            ErrorMessage = "Please enter the reason for your appointment.")]
        [StringLength(
            200,
            ErrorMessage =
                "The reason cannot exceed 200 characters.")]
        [Display(Name = "Reason")]
        public string Reason
        {
            get;
            set;
        }

        [StringLength(
            1000,
            ErrorMessage =
                "Notes cannot exceed 1000 characters.")]
        [Display(Name = "Additional Notes")]
        public string Notes
        {
            get;
            set;
        }
    }
}
