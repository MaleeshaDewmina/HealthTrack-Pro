using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthTrackPro.Web.Models.Profiles
{
    public class TrainerClient
    {
        [Key]
        public int TrainerClientId { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("Trainer")]
        public string TrainerUserId { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("Client")]
        public string ClientUserId { get; set; }

        public DateTime AssignedAt { get; set; }

        public bool IsActive { get; set; }

        public virtual TrainerProfile Trainer { get; set; }

        public virtual ClientProfile Client { get; set; }
    }
}