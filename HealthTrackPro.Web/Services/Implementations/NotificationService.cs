using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using HealthTrackPro.Web.Enums;
using HealthTrackPro.Web.Models;
using HealthTrackPro.Web.Models.Communication;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Services.Implementations
{
    public class NotificationService :
        INotificationService,
        IDisposable
    {
        private readonly ApplicationDbContext db;
        private readonly bool ownsContext;

        public NotificationService()
            : this(new ApplicationDbContext(), true)
        {
        }

        public NotificationService(
            ApplicationDbContext context)
            : this(context, false)
        {
        }

        private NotificationService(
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
        }

        public async Task<List<Notification>>
            GetUserNotificationsAsync(
                string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new List<Notification>();
            }

            return await db.Notifications
                .Where(n =>
                    n.UserId == userId)
                .OrderByDescending(
                    n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>>
            GetUnreadNotificationsAsync(
                string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new List<Notification>();
            }

            return await db.Notifications
                .Where(n =>
                    n.UserId == userId &&
                    !n.IsRead)
                .OrderByDescending(
                    n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int>
            GetUnreadCountAsync(
                string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return 0;
            }

            return await db.Notifications
                .CountAsync(n =>
                    n.UserId == userId &&
                    !n.IsRead);
        }

        public async Task<Notification> CreateAsync(
            string userId,
            string title,
            string message,
            NotificationType type)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException(
                    "User ID is required.",
                    "userId");
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException(
                    "Notification title is required.",
                    "title");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(
                    "Notification message is required.",
                    "message");
            }

            var notification =
                new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    Type = type,
                    CreatedAt =
                        DateTime.UtcNow
                };

            db.Notifications.Add(
                notification);

            await db.SaveChangesAsync();

            return notification;
        }

        public async Task<bool>
            MarkAsReadAsync(
                int notificationId,
                string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            var notification =
                await db.Notifications
                    .FirstOrDefaultAsync(n =>
                        n.NotificationId ==
                            notificationId &&
                        n.UserId ==
                            userId);

            if (notification == null)
            {
                return false;
            }

            if (!notification.IsRead)
            {
                notification.MarkAsRead();

                await db.SaveChangesAsync();
            }

            return true;
        }

        public async Task<int>
            MarkAllAsReadAsync(
                string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return 0;
            }

            var notifications =
                await db.Notifications
                    .Where(n =>
                        n.UserId ==
                            userId &&
                        !n.IsRead)
                    .ToListAsync();

            foreach (var notification
                in notifications)
            {
                notification.MarkAsRead();
            }

            if (notifications.Count > 0)
            {
                await db.SaveChangesAsync();
            }

            return notifications.Count;
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
