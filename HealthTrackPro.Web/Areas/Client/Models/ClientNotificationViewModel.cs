using System.Collections.Generic;

namespace HealthTrackPro.Web.Areas.Client.Models
{
    public class ClientNotificationRowViewModel
    {
        public int NotificationId { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public string CreatedAt { get; set; }

        public string Type { get; set; }

        public bool IsRead { get; set; }
    }

    public class ClientNotificationIndexViewModel
    {
        public ClientNotificationIndexViewModel()
        {
            Items =
                new List<ClientNotificationRowViewModel>();
        }

        public List<ClientNotificationRowViewModel>
            Items { get; set; }

        public int UnreadCount { get; set; }
    }
}