using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using HealthTrackPro.Web.Enums;
using HealthTrackPro.Web.Models;
using HealthTrackPro.Web.Models.Fitness;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Services.Implementations
{
    public class ProgressService :
        IProgressService,
        IDisposable
    {
        private readonly ApplicationDbContext db;
        private readonly bool ownsContext;

        public ProgressService()
            : this(new ApplicationDbContext(), true)
        {
        }

        public ProgressService(
            ApplicationDbContext context)
            : this(context, false)
        {
        }

        private ProgressService(
            ApplicationDbContext context,
            bool ownsContext)
        {
            if (context == null)
            {
                throw new ArgumentNullException(
                    "context");
            }

            db = context;
            this.ownsContext = ownsContext;
        }

        public async Task<List<FitnessGoal>>
            GetGoalsAsync(
                string clientUserId)
        {
            if (string.IsNullOrWhiteSpace(clientUserId))
            {
                return new List<FitnessGoal>();
            }

            return await db.FitnessGoals
                .Where(g =>
                    g.ClientUserId == clientUserId)
                .OrderBy(g => g.IsCompleted)
                .ThenByDescending(g => g.StartDate)
                .ToListAsync();
        }

        public async Task<FitnessGoal>
            CreateGoalAsync(
                string clientUserId,
                FitnessGoalType goalType,
                string title,
                string description,
                decimal? startValue,
                decimal? targetValue,
                DateTime? targetDate)
        {
            if (string.IsNullOrWhiteSpace(clientUserId) ||
                string.IsNullOrWhiteSpace(title))
            {
                return null;
            }

            var clientExists =
                await db.ClientProfiles.AnyAsync(c =>
                    c.UserId == clientUserId);

            if (!clientExists)
            {
                return null;
            }

            var goal =
                new FitnessGoal
                {
                    ClientUserId = clientUserId,
                    GoalType = goalType,
                    Title = title,
                    Description = description,
                    StartValue = startValue,
                    CurrentValue = startValue,
                    TargetValue = targetValue,
                    StartDate = DateTime.UtcNow,
                    TargetDate = targetDate,
                    IsCompleted = false
                };

            db.FitnessGoals.Add(goal);

            await db.SaveChangesAsync();

            return goal;
        }

        public async Task<bool>
            UpdateGoalValueAsync(
                int fitnessGoalId,
                string clientUserId,
                decimal currentValue)
        {
            var goal =
                await db.FitnessGoals
                    .FirstOrDefaultAsync(g =>
                        g.FitnessGoalId ==
                            fitnessGoalId &&
                        g.ClientUserId ==
                            clientUserId);

            if (goal == null)
            {
                return false;
            }

            goal.UpdateCurrentValue(
                currentValue);

            if (!goal.IsCompleted &&
                goal.CalculateProgress() >= 100m)
            {
                goal.MarkAsCompleted();
            }

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool>
            MarkGoalCompletedAsync(
                int fitnessGoalId,
                string clientUserId)
        {
            var goal =
                await db.FitnessGoals
                    .FirstOrDefaultAsync(g =>
                        g.FitnessGoalId ==
                            fitnessGoalId &&
                        g.ClientUserId ==
                            clientUserId);

            if (goal == null)
            {
                return false;
            }

            if (!goal.IsCompleted)
            {
                goal.MarkAsCompleted();

                await db.SaveChangesAsync();
            }

            return true;
        }

        public async Task<List<HealthMetric>>
            GetHealthMetricsAsync(
                string clientUserId)
        {
            if (string.IsNullOrWhiteSpace(clientUserId))
            {
                return new List<HealthMetric>();
            }

            return await db.HealthMetrics
                .Where(h =>
                    h.ClientUserId == clientUserId)
                .OrderByDescending(h => h.RecordedAt)
                .ToListAsync();
        }

        public async Task<HealthMetric>
            GetLatestHealthMetricAsync(
                string clientUserId)
        {
            if (string.IsNullOrWhiteSpace(clientUserId))
            {
                return null;
            }

            return await db.HealthMetrics
                .Where(h =>
                    h.ClientUserId == clientUserId)
                .OrderByDescending(h => h.RecordedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<HealthMetric>
            AddHealthMetricAsync(
                string clientUserId,
                decimal? weightKg,
                decimal? heightCm,
                int? caloriesBurned,
                int? steps,
                decimal? sleepHours,
                decimal? waterLitres)
        {
            if (string.IsNullOrWhiteSpace(clientUserId))
            {
                return null;
            }

            var clientExists =
                await db.ClientProfiles.AnyAsync(c =>
                    c.UserId == clientUserId);

            if (!clientExists)
            {
                return null;
            }

            var metric =
                new HealthMetric
                {
                    ClientUserId = clientUserId,
                    WeightKg = weightKg,
                    HeightCm = heightCm,
                    CaloriesBurned = caloriesBurned,
                    Steps = steps,
                    SleepHours = sleepHours,
                    WaterLitres = waterLitres,
                    RecordedAt = DateTime.UtcNow
                };

            db.HealthMetrics.Add(metric);

            await db.SaveChangesAsync();

            return metric;
        }

        public async Task<decimal?>
            GetCurrentBmiAsync(
                string clientUserId)
        {
            var latest =
                await GetLatestHealthMetricAsync(
                    clientUserId);

            if (latest == null)
            {
                return null;
            }

            return latest.CalculateBMI();
        }

        public void Dispose()
        {
            if (ownsContext)
            {
                db.Dispose();
            }
        }
    }
}
