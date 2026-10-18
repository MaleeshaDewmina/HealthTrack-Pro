
using System;
using System.ComponentModel.DataAnnotations;
using HealthTrackPro.Web.Enums;

namespace HealthTrackPro.Web.Areas.Client.Models
{
    public class ClientProfileViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Range(50, 250,
            ErrorMessage = "Height must be between 50 and 250 cm.")]
        [Display(Name = "Height (cm)")]
        public decimal? HeightCm { get; set; }

        [Range(20, 500,
            ErrorMessage = "Weight must be between 20 and 500 kg.")]
        [Display(Name = "Weight (kg)")]
        public decimal? CurrentWeightKg { get; set; }

        [Display(Name = "Activity Level")]
        public ActivityLevel? ActivityLevel { get; set; }

        [Display(Name = "Primary Fitness Goal")]
        public FitnessGoalType? PrimaryGoal { get; set; }
    }
}
