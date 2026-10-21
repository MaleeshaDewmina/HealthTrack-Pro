using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;

using HealthTrackPro.Web.Areas.Client.Models;
using HealthTrackPro.Web.Services.Implementations;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Areas.Client.Controllers
{
    [Authorize(Roles = "Client")]
    public class NotificationsController : Controller
    {
        private readonly INotificationService
            notificationService;

        public NotificationsController()
        {
            notificationService =
                new NotificationService();
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string userId =
                User.Identity.GetUserId();

            var items =
                await notificationService
                    .GetUserNotificationsAsync(
                        userId);

            var model =
                new ClientNotificationIndexViewModel
                {
                    UnreadCount =
                        items.Count(
                            n => !n.IsRead)
                };

            foreach (
                var notification
                in items.OrderByDescending(
                    n => n.CreatedAt))
            {
                model.Items.Add(
                    new ClientNotificationRowViewModel
                    {
                        NotificationId =
                            notification.NotificationId,

                        Title =
                            notification.Title,

                        Message =
                            notification.Message,

                        CreatedAt =
                            notification.CreatedAt
                                .ToString(
                                    "MMM d, yyyy h:mm tt"),

                        Type =
                            notification.Type.ToString(),

                        IsRead =
                            notification.IsRead
                    });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>
            MarkRead(int id)
        {
            bool updated =
                await notificationService
                    .MarkAsReadAsync(
                        id,
                        User.Identity.GetUserId());

            TempData[
                updated
                    ? "SuccessMessage"
                    : "ErrorMessage"
            ] =
                updated
                    ? "Notification marked as read."
                    : "That notification could not be updated.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>
            MarkAllRead()
        {
            int count =
                await notificationService
                    .MarkAllAsReadAsync(
                        User.Identity.GetUserId());

            TempData["SuccessMessage"] =
                count +
                " notification(s) marked as read.";

            return RedirectToAction("Index");
        }

        protected override void Dispose(
            bool disposing)
        {
            if (disposing)
            {
                (
                    notificationService
                    as IDisposable
                )?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}