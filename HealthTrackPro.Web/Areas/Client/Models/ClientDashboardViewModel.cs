
namespace HealthTrackPro.Web.Areas.Client.Models
{
    public class ClientDashboardViewModel
    {
        public string FullName { get; set; }

        public decimal? CurrentWeightKg { get; set; }

        public decimal? HeightCm { get; set; }

        public decimal? BMI { get; set; }

        // Average measured progress across all saved goals
        public decimal? GoalProgressPercent { get; set; }

        // Total number of saved fitness goals
        public int TotalGoals { get; set; }

        // Number of goals marked as completed
        public int CompletedGoals { get; set; }
    }
}
