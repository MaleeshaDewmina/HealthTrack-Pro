using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using HealthTrackPro.Web.Areas.Admin.Models;
using HealthTrackPro.Web.Models;
using HealthTrackPro.Web.Services.Implementations;
using HealthTrackPro.Web.Services.Interfaces;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace HealthTrackPro.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db =
            new ApplicationDbContext();

        private readonly IAuditService auditService =
            new AuditService();

        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext
                    .GetOwinContext()
                    .GetUserManager<
                        ApplicationUserManager>();
            }
        }

        public async Task<ActionResult> Index(
            string search,
            string role,
            string status)
        {
            var query =
                db.Users
                    .AsNoTracking()
                    .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.Email.Contains(search) ||
                    u.UserName.Contains(search));
            }

            var users =
                await query
                    .OrderBy(u => u.Email)
                    .ToListAsync();

            var items =
                new List<
                    AdminUserListItemViewModel>();

            foreach (var user in users)
            {
                var roles =
                    await UserManager
                        .GetRolesAsync(user.Id);

                var userRole =
                    roles.FirstOrDefault()
                    ?? "Unknown";

                string fullName = user.Email;

                var clientProfile =
                    await db.ClientProfiles
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c =>
                            c.UserId == user.Id);

                if (clientProfile != null)
                {
                    fullName =
                        clientProfile.FullName;
                }

                var trainerProfile =
                    await db.TrainerProfiles
                        .AsNoTracking()
                        .FirstOrDefaultAsync(t =>
                            t.UserId == user.Id);

                if (trainerProfile != null)
                {
                    fullName =
                        trainerProfile.FullName;
                }

                bool suspended =
                    user.LockoutEndDateUtc
                        .HasValue &&
                    user.LockoutEndDateUtc.Value >
                        DateTime.UtcNow;

                items.Add(
                    new AdminUserListItemViewModel
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        FullName = fullName,
                        Role = userRole,
                        IsSuspended = suspended,
                        LockoutEndDateUtc =
                            user.LockoutEndDateUtc
                    });
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                items =
                    items
                        .Where(x =>
                            string.Equals(
                                x.Role,
                                role,
                                StringComparison
                                    .OrdinalIgnoreCase))
                        .ToList();
            }

            if (status == "Active")
            {
                items =
                    items
                        .Where(x =>
                            !x.IsSuspended)
                        .ToList();
            }
            else if (status == "Suspended")
            {
                items =
                    items
                        .Where(x =>
                            x.IsSuspended)
                        .ToList();
            }

            var model =
                new AdminUsersIndexViewModel
                {
                    SearchTerm = search,
                    RoleFilter = role,
                    StatusFilter = status,
                    Users = items
                };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Suspend(
            string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Index");
            }

            var currentUserId =
                User.Identity.GetUserId();

            if (id == currentUserId)
            {
                TempData["ErrorMessage"] =
                    "You cannot suspend your own Administrator account.";

                return RedirectToAction("Index");
            }

            var user =
                await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                TempData["ErrorMessage"] =
                    "User account was not found.";

                return RedirectToAction("Index");
            }

            await UserManager
                .SetLockoutEnabledAsync(
                    id,
                    true);

            await UserManager
                .SetLockoutEndDateAsync(
                    id,
                    DateTimeOffset.UtcNow
                        .AddYears(100));

            await auditService.LogAsync(
                currentUserId,
                "Suspend User",
                "ApplicationUser",
                id,
                "Administrator suspended " +
                    user.Email,
                Request.UserHostAddress);

            TempData["SuccessMessage"] =
                "User suspended successfully.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Reactivate(
            string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Index");
            }

            var user =
                await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                TempData["ErrorMessage"] =
                    "User account was not found.";

                return RedirectToAction("Index");
            }

            await UserManager
                .SetLockoutEndDateAsync(
                    id,
                    DateTimeOffset.UtcNow
                        .AddMinutes(-1));

            await auditService.LogAsync(
                User.Identity.GetUserId(),
                "Reactivate User",
                "ApplicationUser",
                id,
                "Administrator reactivated " +
                    user.Email,
                Request.UserHostAddress);

            TempData["SuccessMessage"] =
                "User reactivated successfully.";

            return RedirectToAction("Index");
        }

        protected override void Dispose(
            bool disposing)
        {
            if (disposing)
            {
                db.Dispose();

                var disposable =
                    auditService as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
