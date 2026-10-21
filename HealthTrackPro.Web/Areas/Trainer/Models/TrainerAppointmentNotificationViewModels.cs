using System.Collections.Generic;

namespace HealthTrackPro.Web.Areas.Trainer.Models
{
    public class AppointmentRowViewModel
    {
        public int AppointmentId { get; set; }
        public string ClientName { get; set; }
        public string When { get; set; }
        public int DurationMinutes { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public bool IsPending { get; set; }
        public bool IsApproved { get; set; }
        public bool CanReschedule { get; set; }
    }

    public class AppointmentIndexViewModel
    {
        public AppointmentIndexViewModel()
        {
            Appointments = new List<AppointmentRowViewModel>();
        }

        public List<AppointmentRowViewModel> Appointments { get; set; }
    }

    public class NotificationRowViewModel
    {
        public int NotificationId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string CreatedAt { get; set; }
        public string Type { get; set; }
        public bool IsRead { get; set; }
    }

    public class NotificationIndexViewModel
    {
        public NotificationIndexViewModel()
        {
            Items = new List<NotificationRowViewModel>();
        }

        public List<NotificationRowViewModel> Items { get; set; }
        public int UnreadCount { get; set; }
    }
}
