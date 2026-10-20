
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using HealthTrackPro.Web.Models.Fitness;

namespace HealthTrackPro.Web.Areas.Client.Models
{
    public class ClientProgressViewModel
    {
        public List<HealthMetric> Metrics { get; set; }

        public List<FitnessGoal> Goals { get; set; }

        public HealthMetric LatestMetric { get; set; }

        public ClientHealthMetricInputViewModel NewMetric
        {
            get;
            set;
        }

        public ClientProgressViewModel()
        {
            Metrics = new List<HealthMetric>();

            Goals = new List<FitnessGoal>();

            NewMetric =
                new ClientHealthMetricInputViewModel();
        }
    }

    public class ClientHealthMetricInputViewModel :
        IValidatableObject
    {
        [Range(
            typeof(decimal),
            "20",
            "500",
            ErrorMessage =
                "Weight must be between 20 and 500 kg.")]
        [Display(Name = "Weight (kg)")]
        public decimal? WeightKg { get; set; }

        [Range(
            typeof(decimal),
            "50",
            "250",
            ErrorMessage =
                "Height must be between 50 and 250 cm.")]
        [Display(Name = "Height (cm)")]
        public decimal? HeightCm { get; set; }

        [Range(
            0,
            10000,
            ErrorMessage =
                "Calories burned must be between 0 and 10,000.")]
        [Display(Name = "Calories Burned")]
        public int? CaloriesBurned { get; set; }

        [Range(
            0,
            100000,
            ErrorMessage =
                "Steps must be between 0 and 100,000.")]
        [Display(Name = "Steps")]
        public int? Steps { get; set; }

        [Range(
            typeof(decimal),
            "0",
            "24",
            ErrorMessage =
                "Sleep hours must be between 0 and 24.")]
        [Display(Name = "Sleep (hours)")]
        public decimal? SleepHours { get; set; }

        [Range(
            typeof(decimal),
            "0",
            "10",
            ErrorMessage =
                "Water intake must be between 0 and 10 litres.")]
        [Display(Name = "Water (litres)")]
        public decimal? WaterLitres { get; set; }

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (!WeightKg.HasValue &&
                !HeightCm.HasValue &&
                !CaloriesBurned.HasValue &&
                !Steps.HasValue &&
                !SleepHours.HasValue &&
                !WaterLitres.HasValue)
            {
                yield return new ValidationResult(
                    "Please enter at least one health measurement.",
                    new[]
                    {
                        "WeightKg",
                        "HeightCm",
                        "CaloriesBurned",
                        "Steps",
                        "SleepHours",
                        "WaterLitres"
                    });
            }
        }
    }
}
