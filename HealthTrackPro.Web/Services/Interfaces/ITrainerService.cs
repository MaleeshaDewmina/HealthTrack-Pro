using System.Collections.Generic;
using System.Threading.Tasks;

using HealthTrackPro.Web.Models.Profiles;

namespace HealthTrackPro.Web.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<TrainerProfile> GetProfileAsync(
            string trainerUserId);

        Task<List<ClientProfile>>
            GetAssignedClientsAsync(
                string trainerUserId);

        Task<bool> IsClientAssignedAsync(
            string trainerUserId,
            string clientUserId);

        Task<bool> AssignClientAsync(
            string trainerUserId,
            string clientUserId);

        Task<bool> UnassignClientAsync(
            string trainerUserId,
            string clientUserId);
    }
}
