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
