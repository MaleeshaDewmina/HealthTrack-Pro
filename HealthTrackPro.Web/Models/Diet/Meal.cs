using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthTrackPro.Web.Models.Diet
{
    public class Meal
    {
        [Key]
        public int MealId { get; set; }

        [Required]
        [ForeignKey("DietPlan")]
        public int DietPlanId { get; set; }

        [Required]
        [StringLength(100)]
        public string MealType { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

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

        public int DisplayOrder { get; set; }

        public virtual DietPlan DietPlan { get; set; }
    }
}
