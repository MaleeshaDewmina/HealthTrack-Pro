using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HealthTrackPro.Web.Areas.Trainer.Models
{
    public class AvailabilitySlotRowViewModel
    {
        public int SlotId { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Notes { get; set; }
        public bool IsOpen { get; set; }
    }

    public class AvailabilityCreateViewModel
    {
        public AvailabilityCreateViewModel()
        {
            Date = DateTime.Today;
        }

        [Required(ErrorMessage = "Choose a date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Choose a start time.")]
        [Display(Name = "Start time")]
        public string StartTime { get; set; }

        [Required(ErrorMessage = "Choose an end time.")]
        [Display(Name = "End time")]
        public string EndTime { get; set; }

        [StringLength(300, ErrorMessage = "Notes must be 300 characters or fewer.")]
        [Display(Name = "Notes (optional)")]
        public string Notes { get; set; }
    }

    public class AvailabilityIndexViewModel
    {
        public AvailabilityIndexViewModel()
        {
            Slots = new List<AvailabilitySlotRowViewModel>();
            Form = new AvailabilityCreateViewModel();
        }

        public List<AvailabilitySlotRowViewModel> Slots { get; set; }
        public AvailabilityCreateViewModel Form { get; set; }
    }

    public class FeedbackRowViewModel
    {
        public string ClientName { get; set; }
        public string RelatedArea { get; set; }
        public string FeedbackText { get; set; }
        public string Rating { get; set; }
        public string NextFocus { get; set; }
        public string CreatedAt { get; set; }
    }

    public class FeedbackIndexViewModel
    {
        public FeedbackIndexViewModel()
        {
            Items = new List<FeedbackRowViewModel>();
        }

        public List<FeedbackRowViewModel> Items { get; set; }
    }

    /// <summary>Form to give feedback to one assigned client.</summary>
    public class FeedbackCreateViewModel
    {
        public FeedbackCreateViewModel()
        {
            Clients = new List<SelectListItem>();
        }

        [Required(ErrorMessage = "Choose a client.")]
        [Display(Name = "Client")]
        public string ClientUserId { get; set; }

        [StringLength(100, ErrorMessage = "Area must be 100 characters or fewer.")]
        [Display(Name = "Related area (optional, e.g. Workout, Diet, Progress)")]
        public string RelatedArea { get; set; }

        [Required(ErrorMessage = "Write your feedback.")]
        [StringLength(1500, ErrorMessage = "Feedback must be 1500 characters or fewer.")]
        [Display(Name = "Feedback")]
        public string FeedbackText { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        [Display(Name = "Rating 1 to 5 (optional)")]
        public int? Rating { get; set; }

        [StringLength(500, ErrorMessage = "Next focus must be 500 characters or fewer.")]
        [Display(Name = "Next focus (optional)")]
        public string NextFocus { get; set; }

        public List<SelectListItem> Clients { get; set; }
    }
}
