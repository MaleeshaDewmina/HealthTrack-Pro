using System.Data.Entity.Migrations;

using HealthTrackPro.Web.Models;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HealthTrackPro.Web.Migrations
{
    internal sealed class Configuration
        : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var roleManager =
                new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));

            string[] roles =
            {
                "Client",
                "Trainer",
                "Administrator"
            };

            foreach (string roleName in roles)
            {
                if (!roleManager.RoleExists(roleName))
                {
                    roleManager.Create(
                        new IdentityRole(roleName));
                }
            }
        }
    }
}