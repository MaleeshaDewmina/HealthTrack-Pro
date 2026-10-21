using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HealthTrackPro.Web.Areas.Trainer.Models
{
    /// <summary>One row in the trainer's workout plan list.</summary>
    public class WorkoutPlanRowViewModel
    {
        public int WorkoutPlanId { get; set; }
        public string ClientName { get; set; }
        public string PlanName { get; set; }
        public string Dates { get; set; }
        public bool IsActive { get; set; }
    }

    public class WorkoutPlanIndexViewModel
    {
        public WorkoutPlanIndexViewModel()
        {
            Plans = new List<WorkoutPlanRowViewModel>();
        }

        public List<WorkoutPlanRowViewModel> Plans { get; set; }
    }

    /// <summary>Form to create a workout plan for one assigned client.</summary>
    public class WorkoutPlanCreateViewModel
    {
        public WorkoutPlanCreateViewModel()
        {
            StartDate = DateTime.Today;
            Clients = new List<SelectListItem>();
        }

        [Required(ErrorMessage = "Choose a client.")]
        [Display(Name = "Client")]
        public string ClientUserId { get; set; }

        [Required(ErrorMessage = "Enter a plan name.")]
        [StringLength(100, ErrorMessage = "Plan name must be 100 characters or fewer.")]
        [Display(Name = "Plan name")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description must be 500 characters or fewer.")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Choose a start date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End date (optional)")]
        public DateTime? EndDate { get; set; }

        public List<SelectListItem> Clients { get; set; }
    }

    public class WorkoutExerciseCreateViewModel
    {
        public WorkoutExerciseCreateViewModel()
        {
            Sets = 3;
            Repetitions = 10;
            RestSeconds = 60;
            DisplayOrder = 1;
        }

        public int WorkoutPlanId { get; set; }

        [Required(ErrorMessage = "Choose an exercise.")]
        [Display(Name = "Exercise")]
        public int? ExerciseId { get; set; }

        [Range(1, 20)]
        public int Sets { get; set; }

        [Range(1, 100)]
        public int Repetitions { get; set; }

        [Range(0, 300)]
        [Display(Name = "Rest seconds")]
        public int RestSeconds { get; set; }

        [Range(0, 300)]
        [Display(Name = "Duration minutes")]
        public int? DurationMinutes { get; set; }

        [Range(1, 100)]
        [Display(Name = "Display order")]
        public int DisplayOrder { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }
    }


    public class WorkoutExerciseRowViewModel
    {
        public int WorkoutExerciseId { get; set; }

        public string ExerciseName { get; set; }

        public string MuscleGroup { get; set; }

        public int Sets { get; set; }

        public int Repetitions { get; set; }

        public int RestSeconds { get; set; }

        public int? DurationMinutes { get; set; }

        public int DisplayOrder { get; set; }

        public string Notes { get; set; }
    }


    public class WorkoutExerciseManageViewModel
    {
        public WorkoutExerciseManageViewModel()
        {
            Exercises =
                new List<WorkoutExerciseRowViewModel>();

            ExerciseOptions =
                new List<SelectListItem>();

            Form =
                new WorkoutExerciseCreateViewModel();
        }

        public int WorkoutPlanId { get; set; }

        public string PlanName { get; set; }

        public string ClientName { get; set; }

        public bool IsActive { get; set; }

        public WorkoutExerciseCreateViewModel Form
        {
            get;
            set;
        }

        public List<SelectListItem> ExerciseOptions
        {
            get;
            set;
        }

        public List<WorkoutExerciseRowViewModel> Exercises
        {
            get;
            set;
        }
    }
}
