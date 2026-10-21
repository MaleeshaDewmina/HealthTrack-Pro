using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using HealthTrackPro.Web.Models.Administration;
using HealthTrackPro.Web.Models.Communication;
using HealthTrackPro.Web.Models.Diet;
using HealthTrackPro.Web.Models.Fitness;
using HealthTrackPro.Web.Models.Profiles;
using HealthTrackPro.Web.Models.Scheduling;
using HealthTrackPro.Web.Models.Workouts;

namespace HealthTrackPro.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(
            UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(
                this,
                DefaultAuthenticationTypes.ApplicationCookie);

            return userIdentity;
        }
    }

    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<ClientProfile> ClientProfiles { get; set; }

        public DbSet<TrainerProfile> TrainerProfiles { get; set; }

        public DbSet<TrainerClient> TrainerClients { get; set; }

        public DbSet<FitnessGoal> FitnessGoals { get; set; }

        public DbSet<HealthMetric> HealthMetrics { get; set; }

        public DbSet<Exercise> Exercises { get; set; }

        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }

        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }

        public DbSet<WorkoutLog> WorkoutLogs { get; set; }

        public DbSet<DietPlan> DietPlans { get; set; }

        public DbSet<Meal> Meals { get; set; }

        public DbSet<MealLog> MealLogs { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<TrainerFeedback> TrainerFeedbacks { get; set; }

        public DbSet<AiConversation> AiConversations { get; set; }

        public DbSet<AiMessage> AiMessages { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(
            DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AiConversation>()
                .HasRequired(c => c.Client)
                .WithMany()
                .HasForeignKey(c => c.ClientUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AiMessage>()
                .HasRequired(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.AiConversationId)
                .WillCascadeOnDelete(true);

            // Prevent accidental cascading deletes across
            // HealthTrack's related domain entities.
            modelBuilder.Conventions.Remove<
                OneToManyCascadeDeleteConvention>();

            modelBuilder.Conventions.Remove<
                ManyToManyCascadeDeleteConvention>();
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
