using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using HealthTrackPro.Web.Models.Workouts;

namespace HealthTrackPro.Web.Services.Interfaces
{
    public interface IWorkoutService
    {
        Task<List<WorkoutPlan>> GetClientPlansAsync(
            string clientUserId);

        Task<List<WorkoutPlan>> GetTrainerPlansAsync(
            string trainerUserId);

        Task<WorkoutPlan> GetActivePlanAsync(
            string clientUserId);

        Task<WorkoutPlan> CreatePlanAsync(
            string clientUserId,
            string trainerUserId,
            string name,
            string description,
            DateTime startDate,
            DateTime? endDate);

        Task<bool> AddExerciseAsync(
            int workoutPlanId,
            string trainerUserId,
            int exerciseId,
            int sets,
            int repetitions,
            int restSeconds,
            int? durationMinutes,
            int displayOrder,
            string notes);

        Task<bool> SetPlanActiveAsync(
            int workoutPlanId,
            string trainerUserId,
            bool isActive);

        Task<WorkoutLog> StartWorkoutAsync(
            string clientUserId,
            int workoutPlanId,
            string notes);

        Task<bool> CompleteWorkoutAsync(
            string clientUserId,
            int workoutLogId,
            int? caloriesBurned);

        Task<List<WorkoutLog>> GetWorkoutHistoryAsync(
            string clientUserId);
    }
}
