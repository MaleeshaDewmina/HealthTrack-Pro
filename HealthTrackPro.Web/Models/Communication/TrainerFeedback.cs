using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthTrackPro.Web.Models.Profiles;

namespace HealthTrackPro.Web.Models.Communication
{
    public class TrainerFeedback
    {
        [Key]
        public int TrainerFeedbackId { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("Trainer")]
        public string TrainerUserId { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("Client")]
        public string ClientUserId { get; set; }

        [StringLength(100)]
        public string RelatedArea { get; set; }

        [Required]
        [StringLength(1500)]
        public string FeedbackText { get; set; }

        [Range(1, 5)]
        public int? Rating { get; set; }

        [StringLength(500)]
        public string NextFocus { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual TrainerProfile Trainer { get; set; }

        public virtual ClientProfile Client { get; set; }
    }
}
