using System.ComponentModel.DataAnnotations;
using HealthTrackPro.Web.Enums;

namespace HealthTrackPro.Web.Models.Workouts
{
    public class Exercise
    {
        [Key]
        public int ExerciseId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string MuscleGroup { get; set; }

        [StringLength(1000)]
        public string Instructions { get; set; }

        public WorkoutDifficulty Difficulty { get; set; }

        public bool IsActive { get; set; }
    }
}
