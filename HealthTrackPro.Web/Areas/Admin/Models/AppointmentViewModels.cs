using System;
using System.Collections.Generic;
using HealthTrackPro.Web.Enums;

namespace HealthTrackPro.Web.Areas.Admin.Models
{
    public class AdminAppointmentListItemViewModel
    {
        public int AppointmentId { get; set; }

        public string ClientName { get; set; }

        public string TrainerName { get; set; }

        public DateTime AppointmentDateTime { get; set; }

        public int DurationMinutes { get; set; }

        public string Reason { get; set; }

        public string Notes { get; set; }

        public AppointmentStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    public class AdminAppointmentsIndexViewModel
    {
        public AdminAppointmentsIndexViewModel()
        {
            Appointments =
                new List<AdminAppointmentListItemViewModel>();
        }

        public string SearchTerm { get; set; }

        public string StatusFilter { get; set; }

        public List<AdminAppointmentListItemViewModel>
            Appointments { get; set; }
    }
}
