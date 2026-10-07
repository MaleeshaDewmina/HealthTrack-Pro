using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthTrackPro.Web.Enums;
using HealthTrackPro.Web.Models.Profiles;

namespace HealthTrackPro.Web.Models.Fitness
{
    public class FitnessGoal
    {
        [Key]
        public int FitnessGoalId { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("Client")]
        public string ClientUserId { get; set; }

        [Required]
        public FitnessGoalType GoalType { get; set; }

        [StringLength(150)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public decimal? StartValue { get; set; }

        public decimal? CurrentValue { get; set; }

        public decimal? TargetValue { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? TargetDate { get; set; }

        public bool IsCompleted { get; set; }

        public virtual ClientProfile Client { get; set; }

        public decimal CalculateProgress()
        {
            if (!StartValue.HasValue ||
                !CurrentValue.HasValue ||
                !TargetValue.HasValue)
            {
                return 0;
            }

            decimal totalDifference = TargetValue.Value - StartValue.Value;

            if (totalDifference == 0)
            {
                return 100;
            }

            decimal currentDifference = CurrentValue.Value - StartValue.Value;

            decimal progress =
                (currentDifference / totalDifference) * 100;

            if (progress < 0)
                return 0;

            if (progress > 100)
                return 100;

            return Math.Round(progress, 2);
        }

        public void MarkAsCompleted()
        {
            IsCompleted = true;
        }

        public void UpdateCurrentValue(decimal value)
        {
            CurrentValue = value;

            if (CalculateProgress() >= 100)
            {
                MarkAsCompleted();
            }
        }
    }
}