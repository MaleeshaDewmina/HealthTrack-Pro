using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HealthTrackPro.Web.Areas.Trainer.Models
{
    /// <summary>One row in the trainer's diet plan list.</summary>
    public class DietPlanRowViewModel
    {
        public int DietPlanId { get; set; }
        public string ClientName { get; set; }
        public string PlanName { get; set; }
        public string Dates { get; set; }
        public string Targets { get; set; }
        public bool IsActive { get; set; }
    }

    public class DietPlanIndexViewModel
    {
        public DietPlanIndexViewModel()
        {
            Plans = new List<DietPlanRowViewModel>();
        }

        public List<DietPlanRowViewModel> Plans { get; set; }
    }

    /// <summary>Form to create a diet plan for one assigned client. Targets are optional.</summary>
    public class DietPlanCreateViewModel
    {
        public DietPlanCreateViewModel()
        {
            StartDate = DateTime.Today;
            Clients = new List<SelectListItem>();
        }

        [Required(ErrorMessage = "Choose a client.")]
        [Display(Name = "Client")]
        public string ClientUserId { get; set; }

        [Required(ErrorMessage = "Enter a plan name.")]
        [StringLength(150, ErrorMessage = "Plan name must be 150 characters or fewer.")]
        [Display(Name = "Plan name")]
        public string Name { get; set; }

        [StringLength(1000, ErrorMessage = "Description must be 1000 characters or fewer.")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Range(500, 10000, ErrorMessage = "Calories must be between 500 and 10000.")]
        [Display(Name = "Daily calories (optional)")]
        public int? DailyCaloriesTarget { get; set; }

        [Range(0, 1000, ErrorMessage = "Protein must be between 0 and 1000 grams.")]
        [Display(Name = "Protein grams (optional)")]
        public decimal? ProteinTargetGrams { get; set; }

        [Range(0, 2000, ErrorMessage = "Carbohydrates must be between 0 and 2000 grams.")]
        [Display(Name = "Carbohydrate grams (optional)")]
        public decimal? CarbohydrateTargetGrams { get; set; }

        [Range(0, 1000, ErrorMessage = "Fat must be between 0 and 1000 grams.")]
        [Display(Name = "Fat grams (optional)")]
        public decimal? FatTargetGrams { get; set; }

        [Required(ErrorMessage = "Choose a start date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End date (optional)")]
        public DateTime? EndDate { get; set; }

        public List<SelectListItem> Clients { get; set; }
    }
}
