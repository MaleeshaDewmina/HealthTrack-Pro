using System;
using System.Collections.Generic;
using HealthTrackPro.Web.Enums;

namespace HealthTrackPro.Web.Areas.Admin.Models
{
    public class AdminDashboardActivityViewModel
    {
        public string UserEmail { get; set; }

        public string Action { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class AdminDashboardAppointmentViewModel
    {
        public int AppointmentId { get; set; }

        public string ClientName { get; set; }

        public string TrainerName { get; set; }

        public DateTime AppointmentDateTime { get; set; }

        public AppointmentStatus Status { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public AdminDashboardViewModel()
        {
            RecentActivities =
                new List<AdminDashboardActivityViewModel>();

            RecentAppointments =
                new List<AdminDashboardAppointmentViewModel>();
        }

        public int TotalClients { get; set; }

        public int TotalTrainers { get; set; }

        public int ActiveAssignments { get; set; }

        public int PendingAppointments { get; set; }

        public int SuspendedUsers { get; set; }

        public int CompletedGoals { get; set; }

        public int ActiveWorkoutPlans { get; set; }

        public int ActiveDietPlans { get; set; }

        public int UnreadNotifications { get; set; }

        public List<AdminDashboardActivityViewModel>
            RecentActivities { get; set; }

        public List<AdminDashboardAppointmentViewModel>
            RecentAppointments { get; set; }
    }
}
