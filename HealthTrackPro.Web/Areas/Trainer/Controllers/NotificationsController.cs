using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using HealthTrackPro.Web.Areas.Trainer.Models;
using HealthTrackPro.Web.Services.Implementations;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Areas.Trainer.Controllers
{
    /// <summary>The logged-in trainer's own notifications.</summary>
    [Authorize(Roles = "Trainer")]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController()
        {
            _notificationService = new NotificationService();
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string userId = User.Identity.GetUserId();
            var items = await _notificationService.GetUserNotificationsAsync(userId);

            var model = new NotificationIndexViewModel
            {
                UnreadCount = items.Count(n => !n.IsRead)
            };

            foreach (var n in items.OrderByDescending(x => x.CreatedAt))
            {
                model.Items.Add(new NotificationRowViewModel
                {
                    NotificationId = n.NotificationId,
                    Title = n.Title,
                    Message = n.Message,
                    CreatedAt = n.CreatedAt.ToString("MMM d, yyyy h:mm tt"),
                    Type = n.Type.ToString(),
                    IsRead = n.IsRead
                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MarkRead(int id)
        {
            bool ok = await _notificationService.MarkAsReadAsync(id, User.Identity.GetUserId());
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok
                ? "Notification marked as read."
                : "That notification could not be updated.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MarkAllRead()
        {
            int count = await _notificationService.MarkAllAsReadAsync(User.Identity.GetUserId());
            TempData["SuccessMessage"] = count + " notification(s) marked as read.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                (_notificationService as IDisposable)?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
