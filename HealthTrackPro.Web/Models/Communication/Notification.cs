using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthTrackPro.Web.Enums;

namespace HealthTrackPro.Web.Models.Communication
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("User")]
        public string UserId { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Message { get; set; }

        public NotificationType Type { get; set; }

        public bool IsRead { get; private set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ReadAt { get; private set; }

        public virtual ApplicationUser User { get; set; }

        public void MarkAsRead()
        {
            if (!IsRead)
            {
                IsRead = true;
                ReadAt = DateTime.Now;
            }
        }
    }
}
