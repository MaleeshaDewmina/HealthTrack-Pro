using System.Collections.Generic;

namespace HealthTrackPro.Web.Areas.Admin.Models
{
    public class AdminTrainerOptionViewModel
    {
        public string UserId { get; set; }

        public string FullName { get; set; }
    }

    public class AdminClientAssignmentViewModel
    {
        public string ClientUserId { get; set; }

        public string ClientName { get; set; }

        public string CurrentTrainerUserId { get; set; }

        public string CurrentTrainerName { get; set; }

        public bool HasTrainer
        {
            get
            {
                return !string.IsNullOrWhiteSpace(
                    CurrentTrainerUserId);
            }
        }
    }

    public class AdminAssignmentsIndexViewModel
    {
        public AdminAssignmentsIndexViewModel()
        {
            Clients =
                new List<
                    AdminClientAssignmentViewModel>();

            Trainers =
                new List<
                    AdminTrainerOptionViewModel>();
        }

        public List<AdminClientAssignmentViewModel>
            Clients { get; set; }

        public List<AdminTrainerOptionViewModel>
            Trainers { get; set; }
    }
}
