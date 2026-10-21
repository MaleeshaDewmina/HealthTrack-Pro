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
}
