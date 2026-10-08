using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using HealthTrackPro.Web.Models.Scheduling;

namespace HealthTrackPro.Web.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<Appointment>> GetClientAppointmentsAsync(
            string clientUserId);

        Task<List<Appointment>> GetTrainerAppointmentsAsync(
            string trainerUserId);

        Task<Appointment> GetByIdAsync(
            int appointmentId);

        Task<bool> BookAsync(
            string clientUserId,
            string trainerUserId,
            DateTime appointmentDateTime,
            int durationMinutes,
            string reason,
            string notes);

        Task<bool> ApproveAsync(
            int appointmentId,
            string trainerUserId);

        Task<bool> RejectAsync(
            int appointmentId,
            string trainerUserId);

        Task<bool> CancelAsync(
            int appointmentId,
            string userId);

        Task<bool> RescheduleAsync(
            int appointmentId,
            string trainerUserId,
            DateTime newDateTime);

        Task<bool> CompleteAsync(
            int appointmentId,
            string trainerUserId);
    }
}
