
using System.Collections.Generic;
using HealthTrackPro.Web.Models.Workouts;

namespace HealthTrackPro.Web.Areas.Client.Models
{
    public class ClientWorkoutViewModel
    {
        public List<WorkoutPlan> Plans { get; set; }

        public WorkoutPlan ActivePlan { get; set; }

        public ClientWorkoutViewModel()
        {
            Plans = new List<WorkoutPlan>();
        }
    }
}
