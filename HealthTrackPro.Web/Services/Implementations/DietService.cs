using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using HealthTrackPro.Web.Models;
using HealthTrackPro.Web.Models.Diet;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Services.Implementations
{
    public class DietService :
        IDietService,
        IDisposable
    {
        private readonly ApplicationDbContext db;
        private readonly bool ownsContext;

        public DietService()
            : this(new ApplicationDbContext(), true)
        {
        }

        public DietService(
            ApplicationDbContext context)
            : this(context, false)
        {
        }

        private DietService(
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

        public async Task<List<DietPlan>>
            GetClientPlansAsync(
                string clientUserId)
        {
            if (string.IsNullOrWhiteSpace(clientUserId))
            {
                return new List<DietPlan>();
            }

            return await db.DietPlans
                .Include(p => p.Trainer)
                .Include(p => p.Meals)
                .Where(p =>
                    p.ClientUserId == clientUserId)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<List<DietPlan>>
            GetTrainerPlansAsync(
                string trainerUserId)
        {
            if (string.IsNullOrWhiteSpace(trainerUserId))
            {
                return new List<DietPlan>();
            }

            return await db.DietPlans
                .Include(p => p.Client)
                .Include(p => p.Meals)
                .Where(p =>
                    p.TrainerUserId == trainerUserId)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<DietPlan>
            GetActivePlanAsync(
                string clientUserId)
        {
            if (string.IsNullOrWhiteSpace(clientUserId))
            {
                return null;
            }

            return await db.DietPlans
                .Include(p => p.Trainer)
                .Include(p => p.Meals)
                .FirstOrDefaultAsync(p =>
                    p.ClientUserId ==
                        clientUserId &&
                    p.IsActive);
        }

        public async Task<DietPlan>
            CreatePlanAsync(
                string clientUserId,
                string trainerUserId,
                string name,
                string description,
                int? dailyCaloriesTarget,
                decimal? proteinTargetGrams,
                decimal? carbohydrateTargetGrams,
                decimal? fatTargetGrams,
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

            var activePlans =
                await db.DietPlans
                    .Where(p =>
                        p.ClientUserId ==
                            clientUserId &&
                        p.IsActive)
                    .ToListAsync();

            foreach (var activePlan in activePlans)
            {
                activePlan.Deactivate();
            }

            var plan =
                new DietPlan
                {
                    ClientUserId = clientUserId,
                    TrainerUserId = trainerUserId,
                    Name = name,
                    Description = description,
                    DailyCaloriesTarget =
                        dailyCaloriesTarget,
                    ProteinTargetGrams =
                        proteinTargetGrams,
                    CarbohydrateTargetGrams =
                        carbohydrateTargetGrams,
                    FatTargetGrams =
                        fatTargetGrams,
                    StartDate = startDate,
                    EndDate = endDate
                };

            plan.Activate();

            db.DietPlans.Add(plan);

            await db.SaveChangesAsync();

            return plan;
        }

        public async Task<bool> AddMealAsync(
            int dietPlanId,
            string trainerUserId,
            string mealType,
            string name,
            string description,
            int calories,
            decimal? proteinGrams,
            decimal? carbohydrateGrams,
            decimal? fatGrams,
            int displayOrder)
        {
            var plan =
                await db.DietPlans
                    .FirstOrDefaultAsync(p =>
                        p.DietPlanId == dietPlanId &&
                        p.TrainerUserId ==
                            trainerUserId);

            if (plan == null ||
                string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(mealType) ||
                calories < 0)
            {
                return false;
            }

            var meal =
                new Meal
                {
                    DietPlanId = dietPlanId,
                    MealType = mealType,
                    Name = name,
                    Description = description,
                    Calories = calories,
                    ProteinGrams = proteinGrams,
                    CarbohydrateGrams =
                        carbohydrateGrams,
                    FatGrams = fatGrams,
                    DisplayOrder = displayOrder
                };

            db.Meals.Add(meal);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SetPlanActiveAsync(
            int dietPlanId,
            string trainerUserId,
            bool isActive)
        {
            var plan =
                await db.DietPlans
                    .FirstOrDefaultAsync(p =>
                        p.DietPlanId == dietPlanId &&
                        p.TrainerUserId ==
                            trainerUserId);

            if (plan == null)
            {
                return false;
            }

            if (isActive)
            {
                var otherPlans =
                    await db.DietPlans
                        .Where(p =>
                            p.ClientUserId ==
                                plan.ClientUserId &&
                            p.DietPlanId !=
                                plan.DietPlanId &&
                            p.IsActive)
                        .ToListAsync();

                foreach (var otherPlan in otherPlans)
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

        public async Task<MealLog> LogMealAsync(
            string clientUserId,
            int? dietPlanId,
            string mealType,
            string foodName,
            string description,
            int calories,
            decimal? proteinGrams,
            decimal? carbohydrateGrams,
            decimal? fatGrams)
        {
            if (string.IsNullOrWhiteSpace(clientUserId) ||
                string.IsNullOrWhiteSpace(mealType) ||
                string.IsNullOrWhiteSpace(foodName) ||
                calories < 0)
            {
                return null;
            }

            if (dietPlanId.HasValue)
            {
                var validPlan =
                    await db.DietPlans.AnyAsync(p =>
                        p.DietPlanId ==
                            dietPlanId.Value &&
                        p.ClientUserId ==
                            clientUserId);

                if (!validPlan)
                {
                    return null;
                }
            }

            var log =
                new MealLog
                {
                    ClientUserId = clientUserId,
                    DietPlanId = dietPlanId,
                    MealType = mealType,
                    FoodName = foodName,
                    Description = description,
                    Calories = calories,
                    ProteinGrams = proteinGrams,
                    CarbohydrateGrams =
                        carbohydrateGrams,
                    FatGrams = fatGrams,
                    LoggedAt = DateTime.UtcNow
                };

            db.MealLogs.Add(log);

            await db.SaveChangesAsync();

            return log;
        }

        public async Task<List<MealLog>>
            GetMealLogsAsync(
                string clientUserId)
        {
            if (string.IsNullOrWhiteSpace(clientUserId))
            {
                return new List<MealLog>();
            }

            return await db.MealLogs
                .Include(m => m.DietPlan)
                .Where(m =>
                    m.ClientUserId == clientUserId)
                .OrderByDescending(m => m.LoggedAt)
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
