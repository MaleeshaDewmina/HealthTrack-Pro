using System;
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

            var userManager =
                new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));

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

            var adminEmail =
                Environment.GetEnvironmentVariable(
                    "HEALTHTRACK_ADMIN_EMAIL");

            var adminPassword =
                Environment.GetEnvironmentVariable(
                    "HEALTHTRACK_ADMIN_PASSWORD");

            if (!string.IsNullOrWhiteSpace(adminEmail) &&
                !string.IsNullOrWhiteSpace(adminPassword))
            {
                var admin =
                    userManager.FindByEmail(adminEmail);

                if (admin == null)
                {
                    admin = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail
                    };

                    var result =
                        userManager.Create(
                            admin,
                            adminPassword);

                    if (!result.Succeeded)
                    {
                        throw new Exception(
                            "Administrator creation failed: " +
                            string.Join(
                                "; ",
                                result.Errors));
                    }
                }

                if (!userManager.IsInRole(
                    admin.Id,
                    "Administrator"))
                {
                    userManager.AddToRole(
                        admin.Id,
                        "Administrator");
                }
            }
        }
    }
}
