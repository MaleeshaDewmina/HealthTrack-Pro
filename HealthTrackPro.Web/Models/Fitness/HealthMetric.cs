using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthTrackPro.Web.Models.Profiles;

namespace HealthTrackPro.Web.Models.Fitness
{
    public class HealthMetric
    {
        [Key]
        public int HealthMetricId { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("Client")]
        public string ClientUserId { get; set; }

        [Range(20, 500)]
        public decimal? WeightKg { get; set; }

        [Range(50, 250)]
        public decimal? HeightCm { get; set; }

        [Range(0, 10000)]
        public int? CaloriesBurned { get; set; }

        [Range(0, 100000)]
        public int? Steps { get; set; }

        [Range(0, 24)]
        public decimal? SleepHours { get; set; }

        [Range(0, 10)]
        public decimal? WaterLitres { get; set; }

        public DateTime RecordedAt { get; set; }

        public virtual ClientProfile Client { get; set; }

        public decimal? CalculateBMI()
        {
            if (!WeightKg.HasValue ||
                !HeightCm.HasValue ||
                HeightCm.Value <= 0)
            {
                return null;
            }

            decimal heightMetres = HeightCm.Value / 100;

            decimal bmi =
                WeightKg.Value /
                (heightMetres * heightMetres);

            return Math.Round(bmi, 2);
        }
    }
}