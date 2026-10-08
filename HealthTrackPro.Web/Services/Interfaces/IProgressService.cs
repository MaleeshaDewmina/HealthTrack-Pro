using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using HealthTrackPro.Web.Enums;
using HealthTrackPro.Web.Models.Fitness;

namespace HealthTrackPro.Web.Services.Interfaces
{
    public interface IProgressService
    {
        Task<List<FitnessGoal>> GetGoalsAsync(
            string clientUserId);

        Task<FitnessGoal> CreateGoalAsync(
            string clientUserId,
            FitnessGoalType goalType,
            string title,
            string description,
            decimal? startValue,
            decimal? targetValue,
            DateTime? targetDate);

        Task<bool> UpdateGoalValueAsync(
            int fitnessGoalId,
            string clientUserId,
            decimal currentValue);

        Task<bool> MarkGoalCompletedAsync(
            int fitnessGoalId,
            string clientUserId);

        Task<List<HealthMetric>> GetHealthMetricsAsync(
            string clientUserId);

        Task<HealthMetric> GetLatestHealthMetricAsync(
            string clientUserId);

        Task<HealthMetric> AddHealthMetricAsync(
            string clientUserId,
            decimal? weightKg,
            decimal? heightCm,
            int? caloriesBurned,
            int? steps,
            decimal? sleepHours,
            decimal? waterLitres);

        Task<decimal?> GetCurrentBmiAsync(
            string clientUserId);
    }
}
