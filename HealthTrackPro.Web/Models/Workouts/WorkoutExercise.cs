using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthTrackPro.Web.Models.Workouts
{
    public class WorkoutExercise
    {
        [Key]
        public int WorkoutExerciseId { get; set; }

        [Required]
        [ForeignKey("WorkoutPlan")]
        public int WorkoutPlanId { get; set; }

        [Required]
        [ForeignKey("Exercise")]
        public int ExerciseId { get; set; }

        [Range(1, 20)]
        public int Sets { get; set; }

        [Range(1, 100)]
        public int Repetitions { get; set; }

        [Range(0, 300)]
        public int RestSeconds { get; set; }

        [Range(0, 300)]
        public int? DurationMinutes { get; set; }

        public int DisplayOrder { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public virtual WorkoutPlan WorkoutPlan { get; set; }

        public virtual Exercise Exercise { get; set; }
    }
}
