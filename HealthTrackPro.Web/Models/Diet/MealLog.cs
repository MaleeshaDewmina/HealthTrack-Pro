using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthTrackPro.Web.Models.Profiles;

namespace HealthTrackPro.Web.Models.Diet
{
    public class MealLog
    {
        [Key]
        public int MealLogId { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("Client")]
        public string ClientUserId { get; set; }

        public int? DietPlanId { get; set; }

        [Required]
        [StringLength(100)]
        public string MealType { get; set; }

        [Required]
        [StringLength(200)]
        public string FoodName { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Range(0, 10000)]
        public int Calories { get; set; }

        [Range(0, 500)]
        public decimal? ProteinGrams { get; set; }

        [Range(0, 1000)]
        public decimal? CarbohydrateGrams { get; set; }

        [Range(0, 500)]
        public decimal? FatGrams { get; set; }

        public DateTime LoggedAt { get; set; }

        public virtual ClientProfile Client { get; set; }

        [ForeignKey("DietPlanId")]
        public virtual DietPlan DietPlan { get; set; }

        public int CalculateTotalMacroCalories()
        {
            decimal proteinCalories =
                (ProteinGrams ?? 0) * 4;

            decimal carbohydrateCalories =
                (CarbohydrateGrams ?? 0) * 4;

            decimal fatCalories =
                (FatGrams ?? 0) * 9;

            return (int)Math.Round(
                proteinCalories +
                carbohydrateCalories +
                fatCalories);
        }
    }
}
