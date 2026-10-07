using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthTrackPro.Web.Models.Profiles
{
    public class TrainerProfile
    {
        [Key]
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [StringLength(100)]
        public string Specialization { get; set; }

        [StringLength(500)]
        public string Bio { get; set; }

        [Range(0, 60)]
        public int YearsOfExperience { get; set; }

        [StringLength(200)]
        public string Qualifications { get; set; }

        public bool IsApproved { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}