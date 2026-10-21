using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HealthTrackPro.Web.Areas.Trainer.Models
{
    public class DietPlanMealRowViewModel
    {
        public string MealType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Calories { get; set; }
        public decimal? ProteinGrams { get; set; }
        public decimal? CarbohydrateGrams { get; set; }
        public decimal? FatGrams { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class DietPlanDetailsViewModel
    {
        public DietPlanDetailsViewModel()
        {
            Meals = new List<DietPlanMealRowViewModel>();
        }

        public int DietPlanId { get; set; }
        public string PlanName { get; set; }
        public string ClientName { get; set; }
        public string Dates { get; set; }
        public string Targets { get; set; }
        public bool IsActive { get; set; }
        public List<DietPlanMealRowViewModel> Meals { get; set; }
        public DietPlanMealCreateViewModel MealForm { get; set; }
    }

    /// <summary>Form to add a meal to a diet plan.</summary>
    public class DietPlanMealCreateViewModel
    {
        public DietPlanMealCreateViewModel()
        {
            DisplayOrder = 1;
            MealTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "Breakfast", Text = "Breakfast" },
                new SelectListItem { Value = "Lunch", Text = "Lunch" },
                new SelectListItem { Value = "Dinner", Text = "Dinner" },
                new SelectListItem { Value = "Snack", Text = "Snack" }
            };
        }

        public int DietPlanId { get; set; }

        [Required(ErrorMessage = "Choose a meal type.")]
        [Display(Name = "Meal type")]
        public string MealType { get; set; }

        [Required(ErrorMessage = "Enter a meal name.")]
        [StringLength(200, ErrorMessage = "Meal name must be 200 characters or fewer.")]
        [Display(Name = "Meal name")]
        public string Name { get; set; }

        [StringLength(1000, ErrorMessage = "Description must be 1000 characters or fewer.")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Enter the calories.")]
        [Range(0, 10000, ErrorMessage = "Calories must be between 0 and 10000.")]
        [Display(Name = "Calories")]
        public int Calories { get; set; }

        [Range(0, 500, ErrorMessage = "Protein must be between 0 and 500 grams.")]
        [Display(Name = "Protein grams (optional)")]
        public decimal? ProteinGrams { get; set; }

        [Range(0, 1000, ErrorMessage = "Carbohydrates must be between 0 and 1000 grams.")]
        [Display(Name = "Carbohydrate grams (optional)")]
        public decimal? CarbohydrateGrams { get; set; }

        [Range(0, 500, ErrorMessage = "Fat must be between 0 and 500 grams.")]
        [Display(Name = "Fat grams (optional)")]
        public decimal? FatGrams { get; set; }

        [Range(1, 100, ErrorMessage = "Order must be between 1 and 100.")]
        [Display(Name = "Display order")]
        public int DisplayOrder { get; set; }

        public List<SelectListItem> MealTypes { get; set; }
    }
}
