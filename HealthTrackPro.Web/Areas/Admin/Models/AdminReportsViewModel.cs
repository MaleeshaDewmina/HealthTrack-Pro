using System;

namespace HealthTrackPro.Web.Areas.Admin.Models
{
    public class AdminReportsViewModel
    {
        // Users
        public int TotalClients { get; set; }
        public int TotalTrainers { get; set; }
        public int SuspendedUsers { get; set; }

        // Trainer / Client relationships
        public int ActiveAssignments { get; set; }
        public int UnassignedClients { get; set; }

        // Appointments
        public int TotalAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int ApprovedAppointments { get; set; }
        public int RejectedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int CompletedAppointments { get; set; }

        // Workout activity
        public int TotalWorkoutPlans { get; set; }
        public int ActiveWorkoutPlans { get; set; }
        public int WorkoutLogs { get; set; }

        // Diet activity
        public int TotalDietPlans { get; set; }
        public int ActiveDietPlans { get; set; }
        public int MealLogs { get; set; }

        // Progress
        public int TotalGoals { get; set; }
        public int CompletedGoals { get; set; }
        public int HealthMeasurements { get; set; }

        // Communication
        public int Notifications { get; set; }
        public int UnreadNotifications { get; set; }

        public decimal GoalCompletionPercentage
        {
            get
            {
                if (TotalGoals <= 0)
                {
                    return 0;
                }

                return Math.Round(
                    (decimal)CompletedGoals /
                    TotalGoals * 100m,
                    1);
            }
        }

        public decimal AssignmentCoveragePercentage
        {
            get
            {
                if (TotalClients <= 0)
                {
                    return 0;
                }

                return Math.Round(
                    (decimal)ActiveAssignments /
                    TotalClients * 100m,
                    1);
            }
        }

        public decimal AppointmentCompletionPercentage
        {
            get
            {
                if (TotalAppointments <= 0)
                {
                    return 0;
                }

                return Math.Round(
                    (decimal)CompletedAppointments /
                    TotalAppointments * 100m,
                    1);
            }
        }
    }
}
