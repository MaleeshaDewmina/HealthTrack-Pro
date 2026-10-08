using System.Collections.Generic;
using System.Threading.Tasks;

using HealthTrackPro.Web.Models.Administration;

namespace HealthTrackPro.Web.Services.Interfaces
{
    public interface IAuditService
    {
        Task<AuditLog> LogAsync(
            string userId,
            string action,
            string entityName,
            string entityId,
            string description,
            string ipAddress);

        Task<List<AuditLog>>
            GetRecentLogsAsync(
                int count);

        Task<List<AuditLog>>
            GetUserLogsAsync(
                string userId,
                int count);
    }
}
