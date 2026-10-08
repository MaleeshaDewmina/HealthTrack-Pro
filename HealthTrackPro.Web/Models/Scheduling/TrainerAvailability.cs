using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthTrackPro.Web.Models.Profiles;

namespace HealthTrackPro.Web.Models.Scheduling
{
    public class TrainerAvailability
    {
        [Key]
        public int TrainerAvailabilityId { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("Trainer")]
        public string TrainerUserId { get; set; }

        public DateTime AvailableDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public bool IsAvailable { get; private set; }

        [StringLength(300)]
        public string Notes { get; set; }

        public virtual TrainerProfile Trainer { get; set; }

        public void OpenSlot()
        {
            if (EndTime <= StartTime)
            {
                throw new InvalidOperationException(
                    "End time must be later than start time.");
            }

            IsAvailable = true;
        }

        public void CloseSlot()
        {
            IsAvailable = false;
        }
    }
}
