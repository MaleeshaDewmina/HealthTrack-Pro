using System.Collections.Generic;
using System.Threading.Tasks;

using HealthTrackPro.Web.Enums;
using HealthTrackPro.Web.Models.Communication;

namespace HealthTrackPro.Web.Services.Interfaces
{
    public interface INotificationService
    {
        Task<List<Notification>>
            GetUserNotificationsAsync(
                string userId);

        Task<List<Notification>>
            GetUnreadNotificationsAsync(
                string userId);

        Task<int> GetUnreadCountAsync(
            string userId);

        Task<Notification> CreateAsync(
            string userId,
            string title,
            string message,
            NotificationType type);

        Task<bool> MarkAsReadAsync(
            int notificationId,
            string userId);

        Task<int> MarkAllAsReadAsync(
            string userId);
    }
}
