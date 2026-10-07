using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthTrackPro.Web.Models.Profiles;

namespace HealthTrackPro.Web.Models.Workouts
{
    public class WorkoutPlan
    {
        public WorkoutPlan()
        {
            WorkoutExercises = new HashSet<WorkoutExercise>();
        }

        [Key]
        public int WorkoutPlanId { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("Client")]
        public string ClientUserId { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("Trainer")]
        public string TrainerUserId { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; private set; }

        public virtual ClientProfile Client { get; set; }

        public virtual TrainerProfile Trainer { get; set; }

        public virtual ICollection<WorkoutExercise> WorkoutExercises { get; set; }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
