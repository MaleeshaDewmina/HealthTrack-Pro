
using System.Collections.Generic;
using HealthTrackPro.Web.Models.Diet;

namespace HealthTrackPro.Web.Areas.Client.Models
{
    public class ClientDietViewModel
    {
        public List<DietPlan> Plans { get; set; }

        public DietPlan ActivePlan { get; set; }

        public ClientDietViewModel()
        {
            Plans = new List<DietPlan>();
        }
    }
}
