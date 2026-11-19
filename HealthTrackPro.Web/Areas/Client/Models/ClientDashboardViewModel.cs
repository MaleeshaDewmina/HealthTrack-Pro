
using System;
using System.Collections.Generic;

namespace HealthTrackPro.Web.Areas.Client.Models
{
    public class ClientDashboardViewModel
    {
        // Client profile information
        public string FullName { get; set; }

        public decimal? CurrentWeightKg { get; set; }

        public decimal? HeightCm { get; set; }

        public decimal? BMI { get; set; }

        // Fitness goal statistics
        public decimal? GoalProgressPercent { get; set; }

        public int TotalGoals { get; set; }

        public int CompletedGoals { get; set; }

        // Historical measurements for the dashboard chart
        public List<ClientHealthTrendPointViewModel> HealthTrend
        { get; set; }

        // HT-VIVA-DASHBOARD: data from actual stored activity (never estimated).
        public int? CaloriesBurnedToday { get; set; }
        public string ActiveWorkoutName { get; set; }
        public string ActiveWorkoutDescription { get; set; }
        public string ActiveWorkoutTrainer { get; set; }
        public int ActiveWorkoutExerciseCount { get; set; }

        public ClientDashboardViewModel()
        {
            HealthTrend =
                new List<ClientHealthTrendPointViewModel>();
        }
    }

    // One historical measurement on the chart
    public class ClientHealthTrendPointViewModel
    {
        public DateTime RecordedAt { get; set; }

        public decimal? WeightKg { get; set; }

        public decimal? BMI { get; set; }
    }
}
