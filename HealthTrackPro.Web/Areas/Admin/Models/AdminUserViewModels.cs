using System;
using System.Collections.Generic;

namespace HealthTrackPro.Web.Areas.Admin.Models
{
    public class AdminUserListItemViewModel
    {
        public string UserId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public bool IsSuspended { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }
    }

    public class AdminUsersIndexViewModel
    {
        public AdminUsersIndexViewModel()
        {
            Users =
                new List<AdminUserListItemViewModel>();
        }

        public string SearchTerm { get; set; }

        public string RoleFilter { get; set; }

        public string StatusFilter { get; set; }

        public List<AdminUserListItemViewModel>
            Users { get; set; }
    }
}
