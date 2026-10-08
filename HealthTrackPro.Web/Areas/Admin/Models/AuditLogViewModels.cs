using System;
using System.Collections.Generic;

namespace HealthTrackPro.Web.Areas.Admin.Models
{
    public class AdminAuditLogItemViewModel
    {
        public int AuditLogId { get; set; }

        public string UserEmail { get; set; }

        public string Action { get; set; }

        public string EntityName { get; set; }

        public string EntityId { get; set; }

        public string Description { get; set; }

        public string IpAddress { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class AdminAuditLogsIndexViewModel
    {
        public AdminAuditLogsIndexViewModel()
        {
            Logs =
                new List<AdminAuditLogItemViewModel>();
        }

        public string SearchTerm { get; set; }

        public string ActionFilter { get; set; }

        public List<string> Actions { get; set; }

        public List<AdminAuditLogItemViewModel>
            Logs { get; set; }
    }
}
