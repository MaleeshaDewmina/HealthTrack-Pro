using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthTrackPro.Web.Models.Administration
{
    public class AuditLog
    {
        [Key]
        public int AuditLogId { get; set; }

        [StringLength(128)]
        [ForeignKey("User")]
        public string UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; }

        [StringLength(100)]
        public string EntityName { get; set; }

        [StringLength(100)]
        public string EntityId { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(50)]
        public string IpAddress { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
