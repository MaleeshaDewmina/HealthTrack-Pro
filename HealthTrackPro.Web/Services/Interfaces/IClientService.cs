using System;
using System.Threading.Tasks;

using HealthTrackPro.Web.Enums;
using HealthTrackPro.Web.Models.Profiles;

namespace HealthTrackPro.Web.Services.Interfaces
{
    public interface IClientService
    {
        Task<ClientProfile> GetProfileAsync(
            string userId);

        Task<TrainerProfile> GetAssignedTrainerAsync(
            string clientUserId);

        Task<bool> HasAssignedTrainerAsync(
            string clientUserId);

        Task<bool> UpdateProfileAsync(
            string userId,
            string fullName,
            DateTime? dateOfBirth,
            decimal? heightCm,
            decimal? currentWeightKg,
            ActivityLevel? activityLevel,
            FitnessGoalType? primaryGoal);
    }
}
