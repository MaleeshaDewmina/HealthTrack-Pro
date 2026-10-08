using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using HealthTrackPro.Web.Enums;
using HealthTrackPro.Web.Models;
using HealthTrackPro.Web.Models.Profiles;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Services.Implementations
{
    public class ClientService : IClientService, IDisposable
    {
        private readonly ApplicationDbContext db;
        private readonly bool ownsContext;

        public ClientService()
            : this(new ApplicationDbContext(), true)
        {
        }

        public ClientService(
            ApplicationDbContext context)
            : this(context, false)
        {
        }

        private ClientService(
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

        public async Task<ClientProfile> GetProfileAsync(
            string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            return await db.ClientProfiles
                .FirstOrDefaultAsync(
                    c => c.UserId == userId);
        }

        public async Task<TrainerProfile>
            GetAssignedTrainerAsync(
                string clientUserId)
        {
            if (string.IsNullOrWhiteSpace(
                clientUserId))
            {
                return null;
            }

            var trainerUserId =
                await db.TrainerClients
                    .Where(tc =>
                        tc.ClientUserId ==
                            clientUserId &&
                        tc.IsActive)
                    .OrderByDescending(
                        tc => tc.AssignedAt)
                    .Select(
                        tc => tc.TrainerUserId)
                    .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(
                trainerUserId))
            {
                return null;
            }

            return await db.TrainerProfiles
                .FirstOrDefaultAsync(
                    t => t.UserId ==
                        trainerUserId);
        }

        public async Task<bool>
            HasAssignedTrainerAsync(
                string clientUserId)
        {
            if (string.IsNullOrWhiteSpace(
                clientUserId))
            {
                return false;
            }

            return await db.TrainerClients
                .AnyAsync(tc =>
                    tc.ClientUserId ==
                        clientUserId &&
                    tc.IsActive);
        }

        public async Task<bool>
            UpdateProfileAsync(
                string userId,
                string fullName,
                DateTime? dateOfBirth,
                decimal? heightCm,
                decimal? currentWeightKg,
                ActivityLevel? activityLevel,
                FitnessGoalType? primaryGoal)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            var profile =
                await db.ClientProfiles
                    .FirstOrDefaultAsync(
                        c => c.UserId ==
                            userId);

            if (profile == null)
            {
                return false;
            }

            profile.FullName = fullName;
            profile.DateOfBirth = dateOfBirth;
            profile.HeightCm = heightCm;
            profile.CurrentWeightKg =
                currentWeightKg;
            profile.ActivityLevel =
                activityLevel;
            profile.PrimaryGoal =
                primaryGoal;

            await db.SaveChangesAsync();

            return true;
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
