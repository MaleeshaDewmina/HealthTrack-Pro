using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthTrackPro.Web.Enums;

namespace HealthTrackPro.Web.Models.Profiles
{
    public class ClientProfile
    {
        [Key]
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Range(50, 250)]
        public decimal? HeightCm { get; set; }

        [Range(20, 500)]
        public decimal? CurrentWeightKg { get; set; }

        public ActivityLevel? ActivityLevel { get; set; }

        public FitnessGoalType? PrimaryGoal { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
