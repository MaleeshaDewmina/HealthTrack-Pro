using System.ComponentModel.DataAnnotations;

namespace HealthTrackPro.Web.Areas.Admin.Models
{
    public class CreateTrainerViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(
            100,
            MinimumLength = 6,
            ErrorMessage =
                "The password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(
            "Password",
            ErrorMessage =
                "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(100)]
        public string Specialization { get; set; }

        [StringLength(500)]
        public string Bio { get; set; }

        [Range(0, 60)]
        [Display(Name = "Years of experience")]
        public int YearsOfExperience { get; set; }

        [StringLength(200)]
        public string Qualifications { get; set; }
    }

    public class TrainerListItemViewModel
    {
        public string UserId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Specialization { get; set; }

        public int YearsOfExperience { get; set; }

        public bool IsApproved { get; set; }
    }
}
