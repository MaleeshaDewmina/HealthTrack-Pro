using System;
using System.Data.Entity.Migrations;

using HealthTrackPro.Web.Models;
using HealthTrackPro.Web.Models.Workouts;

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

            // Default exercise catalogue.
            // AddOrUpdate keeps seeding safe when Update-Database
            // is executed multiple times.
            context.Exercises.AddOrUpdate(
                e => e.Name,

                new Exercise
                {
                    Name = "Push Up",
                    MuscleGroup = "Chest / Triceps",
                    Instructions =
                        "Keep the body straight, lower the chest under control, then press back up.",
                    IsActive = true
                },

                new Exercise
                {
                    Name = "Bodyweight Squat",
                    MuscleGroup = "Legs / Glutes",
                    Instructions =
                        "Keep the chest upright, sit the hips back and stand by driving through the feet.",
                    IsActive = true
                },

                new Exercise
                {
                    Name = "Plank",
                    MuscleGroup = "Core",
                    Instructions =
                        "Maintain a straight body position while bracing the core and glutes.",
                    IsActive = true
                },

                new Exercise
                {
                    Name = "Walking Lunges",
                    MuscleGroup = "Legs / Glutes",
                    Instructions =
                        "Step forward into a controlled lunge and alternate legs while maintaining balance.",
                    IsActive = true
                },

                new Exercise
                {
                    Name = "Bench Press",
                    MuscleGroup = "Chest / Triceps",
                    Instructions =
                        "Lower the weight toward the chest under control and press upward.",
                    IsActive = true
                },

                new Exercise
                {
                    Name = "Shoulder Press",
                    MuscleGroup = "Shoulders / Triceps",
                    Instructions =
                        "Press the weight overhead while keeping the torso stable.",
                    IsActive = true
                },

                new Exercise
                {
                    Name = "Lat Pulldown",
                    MuscleGroup = "Back / Biceps",
                    Instructions =
                        "Pull the bar toward the upper chest while keeping the torso controlled.",
                    IsActive = true
                },

                new Exercise
                {
                    Name = "Dumbbell Curl",
                    MuscleGroup = "Biceps",
                    Instructions =
                        "Curl the dumbbells without swinging and lower them slowly.",
                    IsActive = true
                },

                new Exercise
                {
                    Name = "Tricep Pushdown",
                    MuscleGroup = "Triceps",
                    Instructions =
                        "Keep elbows close to the body and extend the arms under control.",
                    IsActive = true
                },

                new Exercise
                {
                    Name = "Deadlift",
                    MuscleGroup = "Posterior Chain",
                    Instructions =
                        "Maintain a neutral spine and extend the hips and knees together.",
                    IsActive = true
                },

                new Exercise
                {
                    Name = "Running",
                    MuscleGroup = "Cardio",
                    Instructions =
                        "Run at the prescribed pace while maintaining controlled breathing.",
                    IsActive = true
                },

                new Exercise
                {
                    Name = "Cycling",
                    MuscleGroup = "Cardio / Legs",
                    Instructions =
                        "Maintain a steady cadence and suitable resistance.",
                    IsActive = true
                }
            );
        }
    }
}