using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using HealthTrackPro.Web.Models;
using HealthTrackPro.Web.Models.Administration;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Services.Implementations
{
    public class AuditService :
        IAuditService,
        IDisposable
    {
        private readonly ApplicationDbContext db;
        private readonly bool ownsContext;

        public AuditService()
            : this(new ApplicationDbContext(), true)
        {
        }

        public AuditService(
            ApplicationDbContext context)
            : this(context, false)
        {
        }

        private AuditService(
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

        public async Task<AuditLog> LogAsync(
            string userId,
            string action,
            string entityName,
            string entityId,
            string description,
            string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                throw new ArgumentException(
                    "Audit action is required.",
                    "action");
            }

            var log =
                new AuditLog
                {
                    UserId = userId,
                    Action = action,
                    EntityName = entityName,
                    EntityId = entityId,
                    Description = description,
                    IpAddress = ipAddress,
                    CreatedAt =
                        DateTime.UtcNow
                };

            db.AuditLogs.Add(log);

            await db.SaveChangesAsync();

            return log;
        }

        public async Task<List<AuditLog>>
            GetRecentLogsAsync(
                int count)
        {
            count = NormalizeCount(count);

            return await db.AuditLogs
                .Include(a => a.User)
                .OrderByDescending(
                    a => a.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<AuditLog>>
            GetUserLogsAsync(
                string userId,
                int count)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new List<AuditLog>();
            }

            count = NormalizeCount(count);

            return await db.AuditLogs
                .Include(a => a.User)
                .Where(a =>
                    a.UserId == userId)
                .OrderByDescending(
                    a => a.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        private static int NormalizeCount(
            int count)
        {
            if (count <= 0)
            {
                return 50;
            }

            if (count > 500)
            {
                return 500;
            }

            return count;
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
