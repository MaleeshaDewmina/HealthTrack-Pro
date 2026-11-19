using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using HealthTrackPro.Web.Services.Interfaces;
using HealthTrackPro.Web.Services.Implementations;
using Microsoft.AspNet.Identity;

namespace HealthTrackPro.Web.Controllers
{
    /// <summary>
    /// Local UI enhancement. Uses the existing, user-scoped notification service.
    /// This controller never returns notifications belonging to another user.
    /// </summary>
    [Authorize(Roles = "Client,Trainer")]
    public class NotificationDropdownController : Controller
    {
        private readonly INotificationService notificationService;

        public NotificationDropdownController()
        {
            notificationService = new NotificationService();
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public async Task<ActionResult> Recent()
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new HttpStatusCodeResult(401);
            }

            var all = await notificationService.GetUserNotificationsAsync(userId);
            var latest = all
                .OrderByDescending(n => n.CreatedAt)
                .Take(8)
                .Select(n => new
                {
                    id = n.NotificationId,
                    title = n.Title,
                    message = n.Message,
                    type = n.Type.ToString(),
                    isRead = n.IsRead,
                    createdAtUtc = DateTime.SpecifyKind(n.CreatedAt, DateTimeKind.Utc)
                        .ToString("o")
                })
                .ToList();

            return Json(new
            {
                unreadCount = all.Count(n => !n.IsRead),
                items = latest
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MarkRead(int id)
        {
            var userId = User.Identity.GetUserId();
            bool success = await notificationService.MarkAsReadAsync(id, userId);
            if (!success)
            {
                Response.StatusCode = 404;
            }
            return Json(new { success = success });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MarkAllRead()
        {
            var userId = User.Identity.GetUserId();
            int updated = await notificationService.MarkAllAsReadAsync(userId);
            return Json(new { success = true, updated = updated });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                (notificationService as IDisposable)?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
