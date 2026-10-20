using System.ComponentModel.DataAnnotations;

namespace HealthTrackPro.Web.Areas.Client.Models
{
    public class ClientMealLogInputViewModel
    {
        [Required(ErrorMessage = "Please select a meal type.")]
        [StringLength(100)]
        [Display(Name = "Meal Type")]
        public string MealType { get; set; }

        [Required(ErrorMessage = "Please enter a food name.")]
        [StringLength(200)]
        [Display(Name = "Food Name")]
        public string FoodName { get; set; }

        [StringLength(1000)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter the calories.")]
        [Range(0, 10000,
            ErrorMessage = "Calories must be between 0 and 10,000.")]
        [Display(Name = "Calories (kcal)")]
        public int? Calories { get; set; }

        [Range(typeof(decimal), "0", "500",
            ErrorMessage = "Protein must be between 0 and 500 grams.")]
        [Display(Name = "Protein (g)")]
        public decimal? ProteinGrams { get; set; }

        [Range(typeof(decimal), "0", "1000",
            ErrorMessage = "Carbohydrates must be between 0 and 1,000 grams.")]
        [Display(Name = "Carbohydrates (g)")]
        public decimal? CarbohydrateGrams { get; set; }

        [Range(typeof(decimal), "0", "500",
            ErrorMessage = "Fat must be between 0 and 500 grams.")]
        [Display(Name = "Fat (g)")]
        public decimal? FatGrams { get; set; }
    }
}