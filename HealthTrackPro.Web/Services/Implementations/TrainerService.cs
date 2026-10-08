using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using HealthTrackPro.Web.Models;
using HealthTrackPro.Web.Models.Profiles;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Services.Implementations
{
    public class TrainerService :
        ITrainerService,
        IDisposable
    {
        private readonly ApplicationDbContext db;
        private readonly bool ownsContext;

        public TrainerService()
            : this(new ApplicationDbContext(), true)
        {
        }

        public TrainerService(
            ApplicationDbContext context)
            : this(context, false)
        {
        }

        private TrainerService(
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

        public async Task<TrainerProfile>
            GetProfileAsync(
                string trainerUserId)
        {
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

        public async Task<List<ClientProfile>>
            GetAssignedClientsAsync(
                string trainerUserId)
        {
            if (string.IsNullOrWhiteSpace(
                trainerUserId))
            {
                return new List<ClientProfile>();
            }

            var clientIds =
                db.TrainerClients
                    .Where(tc =>
                        tc.TrainerUserId ==
                            trainerUserId &&
                        tc.IsActive)
                    .Select(
                        tc => tc.ClientUserId);

            return await db.ClientProfiles
                .Where(c =>
                    clientIds.Contains(
                        c.UserId))
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<bool>
            IsClientAssignedAsync(
                string trainerUserId,
                string clientUserId)
        {
            if (string.IsNullOrWhiteSpace(
                    trainerUserId) ||
                string.IsNullOrWhiteSpace(
                    clientUserId))
            {
                return false;
            }

            return await db.TrainerClients
                .AnyAsync(tc =>
                    tc.TrainerUserId ==
                        trainerUserId &&
                    tc.ClientUserId ==
                        clientUserId &&
                    tc.IsActive);
        }

        public async Task<bool>
            AssignClientAsync(
                string trainerUserId,
                string clientUserId)
        {
            if (string.IsNullOrWhiteSpace(
                    trainerUserId) ||
                string.IsNullOrWhiteSpace(
                    clientUserId))
            {
                return false;
            }

            var trainerExists =
                await db.TrainerProfiles
                    .AnyAsync(t =>
                        t.UserId ==
                            trainerUserId &&
                        t.IsApproved);

            var clientExists =
                await db.ClientProfiles
                    .AnyAsync(c =>
                        c.UserId ==
                            clientUserId);

            if (!trainerExists ||
                !clientExists)
            {
                return false;
            }

            var alreadyAssigned =
                await db.TrainerClients
                    .AnyAsync(tc =>
                        tc.TrainerUserId ==
                            trainerUserId &&
                        tc.ClientUserId ==
                            clientUserId &&
                        tc.IsActive);

            if (alreadyAssigned)
            {
                return true;
            }

            var currentAssignments =
                await db.TrainerClients
                    .Where(tc =>
                        tc.ClientUserId ==
                            clientUserId &&
                        tc.IsActive)
                    .ToListAsync();

            foreach (var assignment
                in currentAssignments)
            {
                assignment.IsActive = false;
            }

            var newAssignment =
                new TrainerClient
                {
                    TrainerUserId =
                        trainerUserId,

                    ClientUserId =
                        clientUserId,

                    AssignedAt =
                        DateTime.UtcNow,

                    IsActive = true
                };

            db.TrainerClients.Add(
                newAssignment);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool>
            UnassignClientAsync(
                string trainerUserId,
                string clientUserId)
        {
            if (string.IsNullOrWhiteSpace(
                    trainerUserId) ||
                string.IsNullOrWhiteSpace(
                    clientUserId))
            {
                return false;
            }

            var assignment =
                await db.TrainerClients
                    .FirstOrDefaultAsync(tc =>
                        tc.TrainerUserId ==
                            trainerUserId &&
                        tc.ClientUserId ==
                            clientUserId &&
                        tc.IsActive);

            if (assignment == null)
            {
                return false;
            }

            assignment.IsActive = false;

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
