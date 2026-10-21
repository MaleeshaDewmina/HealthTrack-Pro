using System;
using System.Collections.Generic;

namespace HealthTrackPro.Web.Areas.Trainer.Models
{
    public class ProgressMetricPointViewModel
    {
        public string Label { get; set; }
        public decimal? WeightKg { get; set; }
        public decimal? Bmi { get; set; }
        public int? Steps { get; set; }
    }

    public class ProgressClientRowViewModel
    {
        public ProgressClientRowViewModel()
        {
            Trend = new List<ProgressMetricPointViewModel>();
        }

        public string ClientUserId { get; set; }
        public string ClientName { get; set; }
        public string Goal { get; set; }

        public string CurrentWeight { get; set; }
        public decimal? CurrentWeightValue { get; set; }

        public string LastRecorded { get; set; }

        public string Bmi { get; set; }
        public decimal? BmiValue { get; set; }

        public int ActiveGoals { get; set; }
        public int CompletedGoals { get; set; }
        public int TotalGoals { get; set; }

        public string AverageGoalProgress { get; set; }
        public decimal AverageGoalProgressValue { get; set; }

        public int MeasurementCount { get; set; }

        public List<ProgressMetricPointViewModel> Trend { get; set; }
    }

    public class ProgressReportViewModel
    {
        public ProgressReportViewModel()
        {
            Clients = new List<ProgressClientRowViewModel>();
        }

        public List<ProgressClientRowViewModel> Clients { get; set; }

        public DateTime GeneratedAt { get; set; }

        public int TotalActiveGoals { get; set; }
        public int TotalCompletedGoals { get; set; }
        public int ClientsWithMeasurements { get; set; }

        public decimal OverallAverageGoalProgress { get; set; }
    }
}