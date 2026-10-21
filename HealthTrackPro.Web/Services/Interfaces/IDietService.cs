using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using HealthTrackPro.Web.Models.Diet;

namespace HealthTrackPro.Web.Services.Interfaces
{
    public interface IDietService
    {
        Task<List<DietPlan>> GetClientPlansAsync(
            string clientUserId);

        Task<List<DietPlan>> GetTrainerPlansAsync(
            string trainerUserId);

        Task<DietPlan> GetActivePlanAsync(
            string clientUserId);

        Task<DietPlan> CreatePlanAsync(
            string clientUserId,
            string trainerUserId,
            string name,
            string description,
            int? dailyCaloriesTarget,
            decimal? proteinTargetGrams,
            decimal? carbohydrateTargetGrams,
            decimal? fatTargetGrams,
            DateTime startDate,
            DateTime? endDate);

        Task<bool> AddMealAsync(
            int dietPlanId,
            string trainerUserId,
            string mealType,
            string name,
            string description,
            int calories,
            decimal? proteinGrams,
            decimal? carbohydrateGrams,
            decimal? fatGrams,
            int displayOrder);

        Task<bool> SetPlanActiveAsync(
            int dietPlanId,
            string trainerUserId,
            bool isActive);

        Task<MealLog> LogMealAsync(
            string clientUserId,
            int? dietPlanId,
            string mealType,
            string foodName,
            string description,
            int calories,
            decimal? proteinGrams,
            decimal? carbohydrateGrams,
            decimal? fatGrams);

        Task<List<MealLog>> GetMealLogsAsync(
            string clientUserId);
    }
}
