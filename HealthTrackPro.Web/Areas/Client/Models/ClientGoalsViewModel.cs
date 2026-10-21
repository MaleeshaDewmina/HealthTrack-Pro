
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using HealthTrackPro.Web.Enums;
using HealthTrackPro.Web.Models.Fitness;

namespace HealthTrackPro.Web.Areas.Client.Models
{
    // Data displayed on the Client Goals page
    public class ClientGoalsViewModel
    {
        public List<FitnessGoal> Goals { get; set; }

        public CreateFitnessGoalViewModel NewGoal { get; set; }

        public ClientGoalsViewModel()
        {
            Goals = new List<FitnessGoal>();
            NewGoal = new CreateFitnessGoalViewModel();
        }
    }

    // Form used to create a new fitness goal
    public class CreateFitnessGoalViewModel
    {
        [Required(ErrorMessage = "Please select a goal type.")]
        [Display(Name = "Goal Type")]
        public FitnessGoalType? GoalType { get; set; }

        [Required(ErrorMessage = "Please enter a goal title.")]
        [StringLength(150)]
        [Display(Name = "Goal Title")]
        public string Title { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter a starting value.")]
        [Display(Name = "Starting Value")]
        public decimal? StartValue { get; set; }

        [Required(ErrorMessage = "Please enter a target value.")]
        [Display(Name = "Target Value")]
        public decimal? TargetValue { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Target Date")]
        public DateTime? TargetDate { get; set; }
    }

    // Form used to update goal progress
    public class UpdateFitnessGoalValueViewModel
    {
        [Range(1, int.MaxValue)]
        public int FitnessGoalId { get; set; }

        [Required(ErrorMessage = "Please enter your current value.")]
        [Display(Name = "Current Value")]
        public decimal? CurrentValue { get; set; }
    }
}
