using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using HealthTrackPro.Web.Areas.Admin.Models;
using HealthTrackPro.Web.Models;
using HealthTrackPro.Web.Services.Implementations;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AuditLogsController : Controller
    {
        private readonly IAuditService auditService =
            new AuditService();

        private readonly ApplicationDbContext db =
            new ApplicationDbContext();

        public async Task<ActionResult> Index(
            string search,
            string actionFilter)
        {
            var logs =
                await auditService
                    .GetRecentLogsAsync(500);

            var userIds =
                logs
                    .Where(l =>
                        !string.IsNullOrWhiteSpace(
                            l.UserId))
                    .Select(l => l.UserId)
                    .Distinct()
                    .ToList();

            var users =
                await db.Users
                    .AsNoTracking()
                    .Where(u =>
                        userIds.Contains(u.Id))
                    .Select(u => new
                    {
                        u.Id,
                        u.Email
                    })
                    .ToListAsync();

            var userEmails =
                users.ToDictionary(
                    u => u.Id,
                    u => u.Email);

            var actionOptions =
                logs
                    .Where(l =>
                        !string.IsNullOrWhiteSpace(
                            l.Action))
                    .Select(l => l.Action)
                    .Distinct()
                    .OrderBy(a => a)
                    .ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term =
                    search.Trim();

                logs =
                    logs
                        .Where(l =>
                            (
                                !string.IsNullOrWhiteSpace(
                                    l.UserId) &&
                                userEmails.ContainsKey(
                                    l.UserId) &&
                                userEmails[l.UserId]
                                    .IndexOf(
                                        term,
                                        StringComparison
                                            .OrdinalIgnoreCase)
                                    >= 0
                            )
                            ||
                            (
                                l.Action != null &&
                                l.Action.IndexOf(
                                    term,
                                    StringComparison
                                        .OrdinalIgnoreCase)
                                    >= 0
                            )
                            ||
                            (
                                l.EntityName != null &&
                                l.EntityName.IndexOf(
                                    term,
                                    StringComparison
                                        .OrdinalIgnoreCase)
                                    >= 0
                            )
                            ||
                            (
                                l.Description != null &&
                                l.Description.IndexOf(
                                    term,
                                    StringComparison
                                        .OrdinalIgnoreCase)
                                    >= 0
                            ))
                        .ToList();
            }

            if (!string.IsNullOrWhiteSpace(actionFilter))
            {
                logs =
                    logs
                        .Where(l =>
                            string.Equals(
                                l.Action,
                                actionFilter,
                                StringComparison
                                    .OrdinalIgnoreCase))
                        .ToList();
            }

            var model =
                new AdminAuditLogsIndexViewModel
                {
                    SearchTerm =
                        search,

                    ActionFilter =
                        actionFilter,

                    Actions =
                        actionOptions,

                    Logs =
                        logs
                            .Select(l =>
                            {
                                string email =
                                    "System";

                                if (
                                    !string.IsNullOrWhiteSpace(
                                        l.UserId) &&
                                    userEmails.ContainsKey(
                                        l.UserId))
                                {
                                    email =
                                        userEmails[l.UserId];
                                }

                                return
                                    new AdminAuditLogItemViewModel
                                    {
                                        AuditLogId =
                                            l.AuditLogId,

                                        UserEmail =
                                            email,

                                        Action =
                                            l.Action,

                                        EntityName =
                                            l.EntityName,

                                        EntityId =
                                            l.EntityId,

                                        Description =
                                            l.Description,

                                        IpAddress =
                                            l.IpAddress,

                                        CreatedAt =
                                            l.CreatedAt
                                    };
                            })
                            .ToList()
                };

            return View(model);
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





