using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using HealthTrackPro.Web.Models.Profiles;

namespace HealthTrackPro.Web.Areas.Trainer.Models
{
    /// <summary>Data for the Trainer "Assigned Clients" page.</summary>
    public class TrainerClientListViewModel
    {
        public TrainerClientListViewModel()
        {
            Clients = new List<TrainerClientListItemViewModel>();
        }

        public List<TrainerClientListItemViewModel> Clients { get; set; }
    }

    /// <summary>One row of the assigned-clients table. Only the fields the page displays.</summary>
    public class TrainerClientListItemViewModel
    {
        public string ClientUserId { get; set; }
        public string FullName { get; set; }
        public string PrimaryGoal { get; set; }
        public decimal? CurrentWeightKg { get; set; }
        public decimal? Bmi { get; set; }
        public string AssignmentStatus { get; set; }

        /// <summary>Builds a row from the client's profile and their latest BMI.</summary>
        public static TrainerClientListItemViewModel FromProfile(
            ClientProfile profile,
            decimal? bmi)
        {
            if (profile == null)
            {
                throw new ArgumentNullException("profile");
            }

            return new TrainerClientListItemViewModel
            {
                ClientUserId = profile.UserId,
                FullName = string.IsNullOrWhiteSpace(profile.FullName)
                    ? "Unnamed client"
                    : profile.FullName,
                PrimaryGoal = profile.PrimaryGoal.HasValue
                    ? SplitPascalCase(profile.PrimaryGoal.Value.ToString())
                    : "Not set",
                CurrentWeightKg = profile.CurrentWeightKg,
                Bmi = bmi,
                AssignmentStatus = "Active"
            };
        }

        // "LoseWeight" -> "Lose Weight"
        private static string SplitPascalCase(string value)
        {
            return Regex.Replace(value, "(?<!^)([A-Z])", " $1");
        }
    }
}

namespace HealthTrackPro.Web.Areas.Trainer.Models
{
    /// <summary>Client Details page: profile, goals, measurements and plans, all for one assigned client.</summary>
    public class ClientDetailsViewModel
    {
        public ClientDetailsViewModel()
        {
            Goals = new List<ClientGoalRowViewModel>();
            Measurements = new List<ClientMeasurementRowViewModel>();
            WorkoutPlans = new List<ClientPlanRowViewModel>();
            DietPlans = new List<ClientPlanRowViewModel>();
        }

        public string ClientUserId { get; set; }
        public string FullName { get; set; }
        public string PrimaryGoal { get; set; }
        public decimal? CurrentWeightKg { get; set; }
        public decimal? HeightCm { get; set; }
        public decimal? Bmi { get; set; }

        public List<ClientGoalRowViewModel> Goals { get; set; }
        public List<ClientMeasurementRowViewModel> Measurements { get; set; }
        public List<ClientPlanRowViewModel> WorkoutPlans { get; set; }
        public List<ClientPlanRowViewModel> DietPlans { get; set; }
    }

    public class ClientGoalRowViewModel
    {
        public string Title { get; set; }
        public string GoalType { get; set; }
        public decimal? CurrentValue { get; set; }
        public decimal? TargetValue { get; set; }
        public decimal ProgressPercent { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class ClientMeasurementRowViewModel
    {
        public DateTime RecordedAt { get; set; }
        public decimal? WeightKg { get; set; }
        public decimal? Bmi { get; set; }
        public int? Steps { get; set; }
    }

    public class ClientPlanRowViewModel
    {
        public string Name { get; set; }
        public string Detail { get; set; }
        public bool IsActive { get; set; }
    }
}

namespace HealthTrackPro.Web.Areas.Trainer.Models
{
    /// <summary>Trainer dashboard: counts and short lists, all for the logged-in trainer.</summary>
    public class TrainerDashboardViewModel
    {
        public TrainerDashboardViewModel()
        {
            RecentClients = new List<DashboardClientRowViewModel>();
            UpcomingAppointments = new List<DashboardAppointmentRowViewModel>();
            RecentNotifications = new List<DashboardNotificationRowViewModel>();
        }

        public int AssignedClientCount { get; set; }
        public int PendingAppointmentCount { get; set; }
        public int ActiveWorkoutPlanCount { get; set; }
        public int ActiveDietPlanCount { get; set; }
        public int UnreadNotificationCount { get; set; }

        public List<DashboardClientRowViewModel> RecentClients { get; set; }
        public List<DashboardAppointmentRowViewModel> UpcomingAppointments { get; set; }
        public List<DashboardNotificationRowViewModel> RecentNotifications { get; set; }
    }

    public class DashboardClientRowViewModel
    {
        public string ClientUserId { get; set; }
        public string FullName { get; set; }
        public string PrimaryGoal { get; set; }
    }

    public class DashboardAppointmentRowViewModel
    {
        public string ClientName { get; set; }
        public string When { get; set; }
        public string Status { get; set; }
    }

    public class DashboardNotificationRowViewModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string When { get; set; }
        public bool IsRead { get; set; }
    }
}
