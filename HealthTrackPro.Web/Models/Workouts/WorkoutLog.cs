using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthTrackPro.Web.Models.Profiles;

namespace HealthTrackPro.Web.Models.Workouts
{
    public class WorkoutLog
    {
        [Key]
        public int WorkoutLogId { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("Client")]
        public string ClientUserId { get; set; }

        [Required]
        [ForeignKey("WorkoutPlan")]
        public int WorkoutPlanId { get; set; }

        public DateTime StartedAt { get; private set; }

        public DateTime? CompletedAt { get; private set; }

        public int? DurationMinutes { get; private set; }

        [Range(0, 5000)]
        public int? CaloriesBurned { get; private set; }

        public bool IsCompleted { get; private set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public virtual ClientProfile Client { get; set; }

        public virtual WorkoutPlan WorkoutPlan { get; set; }

        public void StartWorkout()
        {
            StartedAt = DateTime.Now;
            IsCompleted = false;
        }

        public void CompleteWorkout(int? caloriesBurned = null)
        {
            CompletedAt = DateTime.Now;

            if (StartedAt != default(DateTime))
            {
                DurationMinutes =
                    (int)Math.Round(
                        (CompletedAt.Value - StartedAt).TotalMinutes);
            }

            CaloriesBurned = caloriesBurned;
            IsCompleted = true;
        }
    }
}
