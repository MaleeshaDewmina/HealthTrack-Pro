using System;
using System.Collections.Generic;

namespace HealthTrackPro.Web.Areas.Trainer.Models
{
    public class ProgressClientRowViewModel
    {
        public string ClientUserId { get; set; }
        public string ClientName { get; set; }
        public string Goal { get; set; }
        public string CurrentWeight { get; set; }
        public string LastRecorded { get; set; }
        public string Bmi { get; set; }
        public int ActiveGoals { get; set; }
        public int CompletedGoals { get; set; }
        public string AverageGoalProgress { get; set; }
    }

    public class ProgressReportViewModel
    {
        public ProgressReportViewModel()
        {
            Clients = new List<ProgressClientRowViewModel>();
        }

        public List<ProgressClientRowViewModel> Clients { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
