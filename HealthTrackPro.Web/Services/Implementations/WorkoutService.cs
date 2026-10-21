using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using HealthTrackPro.Web.Models;
using HealthTrackPro.Web.Models.Workouts;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Services.Implementations
{
    public class WorkoutService :
        IWorkoutService,
        IDisposable
    {
        private readonly ApplicationDbContext db;
        private readonly bool ownsContext;

        public WorkoutService()
            : this(new ApplicationDbContext(), true)
        {
        }

        public WorkoutService(
            ApplicationDbContext context)
            : this(context, false)
        {
        }

        private WorkoutService(
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

        public async Task<List<WorkoutPlan>>
            GetClientPlansAsync(
                string clientUserId)
        {
            if (string.IsNullOrWhiteSpace(clientUserId))
            {
                return new List<WorkoutPlan>();
            }

            return await db.WorkoutPlans
                .Include(p => p.Trainer)
                .Include(
                    p => p.WorkoutExercises
                        .Select(we => we.Exercise))
                .Where(p =>
                    p.ClientUserId == clientUserId)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<List<WorkoutPlan>>
            GetTrainerPlansAsync(
                string trainerUserId)
        {
            if (string.IsNullOrWhiteSpace(trainerUserId))
            {
                return new List<WorkoutPlan>();
            }

            return await db.WorkoutPlans
                .Include(p => p.Client)
                .Include(
                    p => p.WorkoutExercises
                        .Select(we => we.Exercise))
                .Where(p =>
                    p.TrainerUserId == trainerUserId)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<WorkoutPlan>
            GetActivePlanAsync(
                string clientUserId)
        {
            if (string.IsNullOrWhiteSpace(clientUserId))
            {
                return null;
            }

            return await db.WorkoutPlans
                .Include(p => p.Trainer)
                .Include(
                    p => p.WorkoutExercises
                        .Select(we => we.Exercise))
                .FirstOrDefaultAsync(p =>
                    p.ClientUserId == clientUserId &&
                    p.IsActive);
        }

        public async Task<WorkoutPlan>
            CreatePlanAsync(
                string clientUserId,
                string trainerUserId,
                string name,
                string description,
                DateTime startDate,
                DateTime? endDate)
        {
            if (string.IsNullOrWhiteSpace(clientUserId) ||
                string.IsNullOrWhiteSpace(trainerUserId) ||
                string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            if (endDate.HasValue &&
                endDate.Value < startDate)
            {
                return null;
            }

            var assigned =
                await db.TrainerClients.AnyAsync(tc =>
                    tc.ClientUserId == clientUserId &&
                    tc.TrainerUserId == trainerUserId &&
                    tc.IsActive);

            if (!assigned)
            {
                return null;
            }

            var currentPlans =
                await db.WorkoutPlans
                    .Where(p =>
                        p.ClientUserId == clientUserId &&
                        p.IsActive)
                    .ToListAsync();

            foreach (var currentPlan in currentPlans)
            {
                currentPlan.Deactivate();
            }

            var plan =
                new WorkoutPlan
                {
                    ClientUserId = clientUserId,
                    TrainerUserId = trainerUserId,
                    Name = name,
                    Description = description,
                    StartDate = startDate,
                    EndDate = endDate
                };

            plan.Activate();

            db.WorkoutPlans.Add(plan);

            await db.SaveChangesAsync();

            return plan;
        }

        public async Task<List<Exercise>>
            GetAvailableExercisesAsync()
        {
            return await db.Exercises
                .Where(e => e.IsActive)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<bool> AddExerciseAsync(
            int workoutPlanId,
            string trainerUserId,
            int exerciseId,
            int sets,
            int repetitions,
            int restSeconds,
            int? durationMinutes,
            int displayOrder,
            string notes)
        {
            var plan =
                await db.WorkoutPlans
                    .FirstOrDefaultAsync(p =>
                        p.WorkoutPlanId ==
                            workoutPlanId &&
                        p.TrainerUserId ==
                            trainerUserId);

            if (plan == null)
            {
                return false;
            }

            var exerciseExists =
                await db.Exercises.AnyAsync(e =>
                    e.ExerciseId == exerciseId);

            if (!exerciseExists)
            {
                return false;
            }

            if (sets < 0 ||
                repetitions < 0 ||
                restSeconds < 0)
            {
                return false;
            }

            var item =
                new WorkoutExercise
                {
                    WorkoutPlanId = workoutPlanId,
                    ExerciseId = exerciseId,
                    Sets = sets,
                    Repetitions = repetitions,
                    RestSeconds = restSeconds,
                    DurationMinutes = durationMinutes,
                    DisplayOrder = displayOrder,
                    Notes = notes
                };

            db.WorkoutExercises.Add(item);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveExerciseAsync(
            int workoutExerciseId,
            string trainerUserId)
        {
            if (string.IsNullOrWhiteSpace(trainerUserId))
            {
                return false;
            }

            var item =
                await db.WorkoutExercises
                    .Include(we => we.WorkoutPlan)
                    .FirstOrDefaultAsync(we =>
                        we.WorkoutExerciseId ==
                            workoutExerciseId &&
                        we.WorkoutPlan.TrainerUserId ==
                            trainerUserId);

            if (item == null)
            {
                return false;
            }

            db.WorkoutExercises.Remove(item);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SetPlanActiveAsync(
            int workoutPlanId,
            string trainerUserId,
            bool isActive)
        {
            var plan =
                await db.WorkoutPlans
                    .FirstOrDefaultAsync(p =>
                        p.WorkoutPlanId ==
                            workoutPlanId &&
                        p.TrainerUserId ==
                            trainerUserId);

            if (plan == null)
            {
                return false;
            }

            if (isActive)
            {
                var otherActivePlans =
                    await db.WorkoutPlans
                        .Where(p =>
                            p.ClientUserId ==
                                plan.ClientUserId &&
                            p.WorkoutPlanId !=
                                plan.WorkoutPlanId &&
                            p.IsActive)
                        .ToListAsync();

                foreach (var otherPlan
                    in otherActivePlans)
                {
                    otherPlan.Deactivate();
                }

                plan.Activate();
            }
            else
            {
                plan.Deactivate();
            }

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<WorkoutLog>
            StartWorkoutAsync(
                string clientUserId,
                int workoutPlanId,
                string notes)
        {
            var validPlan =
                await db.WorkoutPlans.AnyAsync(p =>
                    p.WorkoutPlanId ==
                        workoutPlanId &&
                    p.ClientUserId ==
                        clientUserId &&
                    p.IsActive);

            if (!validPlan)
            {
                return null;
            }

            var log =
                new WorkoutLog
                {
                    ClientUserId = clientUserId,
                    WorkoutPlanId = workoutPlanId,
                    Notes = notes
                };

            log.StartWorkout();

            db.WorkoutLogs.Add(log);

            await db.SaveChangesAsync();

            return log;
        }

        public async Task<bool>
            CompleteWorkoutAsync(
                string clientUserId,
                int workoutLogId,
                int? caloriesBurned)
        {
            var log =
                await db.WorkoutLogs
                    .FirstOrDefaultAsync(w =>
                        w.WorkoutLogId ==
                            workoutLogId &&
                        w.ClientUserId ==
                            clientUserId);

            if (log == null)
            {
                return false;
            }

            if (log.IsCompleted)
            {
                return true;
            }

            log.CompleteWorkout(
                caloriesBurned);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<List<WorkoutLog>>
            GetWorkoutHistoryAsync(
                string clientUserId)
        {
            if (string.IsNullOrWhiteSpace(clientUserId))
            {
                return new List<WorkoutLog>();
            }

            return await db.WorkoutLogs
                .Include(w => w.WorkoutPlan)
                .Where(w =>
                    w.ClientUserId == clientUserId)
                .OrderByDescending(w => w.StartedAt)
                .ToListAsync();
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
