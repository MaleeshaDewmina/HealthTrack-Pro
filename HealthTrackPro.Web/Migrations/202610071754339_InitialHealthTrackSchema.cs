namespace HealthTrackPro.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialHealthTrackSchema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Appointments",
                c => new
                    {
                        AppointmentId = c.Int(nullable: false, identity: true),
                        ClientUserId = c.String(nullable: false, maxLength: 128),
                        TrainerUserId = c.String(nullable: false, maxLength: 128),
                        AppointmentDateTime = c.DateTime(nullable: false),
                        DurationMinutes = c.Int(nullable: false),
                        Reason = c.String(nullable: false, maxLength: 200),
                        Notes = c.String(maxLength: 1000),
                        Status = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.AppointmentId)
                .ForeignKey("dbo.ClientProfiles", t => t.ClientUserId)
                .ForeignKey("dbo.TrainerProfiles", t => t.TrainerUserId)
                .Index(t => t.ClientUserId)
                .Index(t => t.TrainerUserId);
            
            CreateTable(
                "dbo.ClientProfiles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(nullable: false, maxLength: 100),
                        DateOfBirth = c.DateTime(),
                        HeightCm = c.Decimal(precision: 18, scale: 2),
                        CurrentWeightKg = c.Decimal(precision: 18, scale: 2),
                        ActivityLevel = c.Int(),
                        PrimaryGoal = c.Int(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.TrainerProfiles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Specialization = c.String(maxLength: 100),
                        Bio = c.String(maxLength: 500),
                        YearsOfExperience = c.Int(nullable: false),
                        Qualifications = c.String(maxLength: 200),
                        IsApproved = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AuditLogs",
                c => new
                    {
                        AuditLogId = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        Action = c.String(nullable: false, maxLength: 100),
                        EntityName = c.String(maxLength: 100),
                        EntityId = c.String(maxLength: 100),
                        Description = c.String(maxLength: 1000),
                        IpAddress = c.String(maxLength: 50),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AuditLogId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DietPlans",
                c => new
                    {
                        DietPlanId = c.Int(nullable: false, identity: true),
                        ClientUserId = c.String(nullable: false, maxLength: 128),
                        TrainerUserId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 150),
                        Description = c.String(maxLength: 1000),
                        DailyCaloriesTarget = c.Int(),
                        ProteinTargetGrams = c.Decimal(precision: 18, scale: 2),
                        CarbohydrateTargetGrams = c.Decimal(precision: 18, scale: 2),
                        FatTargetGrams = c.Decimal(precision: 18, scale: 2),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DietPlanId)
                .ForeignKey("dbo.ClientProfiles", t => t.ClientUserId)
                .ForeignKey("dbo.TrainerProfiles", t => t.TrainerUserId)
                .Index(t => t.ClientUserId)
                .Index(t => t.TrainerUserId);
            
            CreateTable(
                "dbo.Meals",
                c => new
                    {
                        MealId = c.Int(nullable: false, identity: true),
                        DietPlanId = c.Int(nullable: false),
                        MealType = c.String(nullable: false, maxLength: 100),
                        Name = c.String(nullable: false, maxLength: 200),
                        Description = c.String(maxLength: 1000),
                        Calories = c.Int(nullable: false),
                        ProteinGrams = c.Decimal(precision: 18, scale: 2),
                        CarbohydrateGrams = c.Decimal(precision: 18, scale: 2),
                        FatGrams = c.Decimal(precision: 18, scale: 2),
                        DisplayOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MealId)
                .ForeignKey("dbo.DietPlans", t => t.DietPlanId)
                .Index(t => t.DietPlanId);
            
            CreateTable(
                "dbo.Exercises",
                c => new
                    {
                        ExerciseId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        MuscleGroup = c.String(maxLength: 100),
                        Instructions = c.String(maxLength: 1000),
                        Difficulty = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ExerciseId);
            
            CreateTable(
                "dbo.FitnessGoals",
                c => new
                    {
                        FitnessGoalId = c.Int(nullable: false, identity: true),
                        ClientUserId = c.String(nullable: false, maxLength: 128),
                        GoalType = c.Int(nullable: false),
                        Title = c.String(maxLength: 150),
                        Description = c.String(maxLength: 500),
                        StartValue = c.Decimal(precision: 18, scale: 2),
                        CurrentValue = c.Decimal(precision: 18, scale: 2),
                        TargetValue = c.Decimal(precision: 18, scale: 2),
                        StartDate = c.DateTime(nullable: false),
                        TargetDate = c.DateTime(),
                        IsCompleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.FitnessGoalId)
                .ForeignKey("dbo.ClientProfiles", t => t.ClientUserId)
                .Index(t => t.ClientUserId);
            
            CreateTable(
                "dbo.HealthMetrics",
                c => new
                    {
                        HealthMetricId = c.Int(nullable: false, identity: true),
                        ClientUserId = c.String(nullable: false, maxLength: 128),
                        WeightKg = c.Decimal(precision: 18, scale: 2),
                        HeightCm = c.Decimal(precision: 18, scale: 2),
                        CaloriesBurned = c.Int(),
                        Steps = c.Int(),
                        SleepHours = c.Decimal(precision: 18, scale: 2),
                        WaterLitres = c.Decimal(precision: 18, scale: 2),
                        RecordedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.HealthMetricId)
                .ForeignKey("dbo.ClientProfiles", t => t.ClientUserId)
                .Index(t => t.ClientUserId);
            
            CreateTable(
                "dbo.MealLogs",
                c => new
                    {
                        MealLogId = c.Int(nullable: false, identity: true),
                        ClientUserId = c.String(nullable: false, maxLength: 128),
                        DietPlanId = c.Int(),
                        MealType = c.String(nullable: false, maxLength: 100),
                        FoodName = c.String(nullable: false, maxLength: 200),
                        Description = c.String(maxLength: 1000),
                        Calories = c.Int(nullable: false),
                        ProteinGrams = c.Decimal(precision: 18, scale: 2),
                        CarbohydrateGrams = c.Decimal(precision: 18, scale: 2),
                        FatGrams = c.Decimal(precision: 18, scale: 2),
                        LoggedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MealLogId)
                .ForeignKey("dbo.ClientProfiles", t => t.ClientUserId)
                .ForeignKey("dbo.DietPlans", t => t.DietPlanId)
                .Index(t => t.ClientUserId)
                .Index(t => t.DietPlanId);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        NotificationId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Title = c.String(nullable: false, maxLength: 150),
                        Message = c.String(nullable: false, maxLength: 1000),
                        Type = c.Int(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        ReadAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.NotificationId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.TrainerAvailabilities",
                c => new
                    {
                        TrainerAvailabilityId = c.Int(nullable: false, identity: true),
                        TrainerUserId = c.String(nullable: false, maxLength: 128),
                        AvailableDate = c.DateTime(nullable: false),
                        StartTime = c.Time(nullable: false, precision: 7),
                        EndTime = c.Time(nullable: false, precision: 7),
                        IsAvailable = c.Boolean(nullable: false),
                        Notes = c.String(maxLength: 300),
                    })
                .PrimaryKey(t => t.TrainerAvailabilityId)
                .ForeignKey("dbo.TrainerProfiles", t => t.TrainerUserId)
                .Index(t => t.TrainerUserId);
            
            CreateTable(
                "dbo.TrainerClients",
                c => new
                    {
                        TrainerClientId = c.Int(nullable: false, identity: true),
                        TrainerUserId = c.String(nullable: false, maxLength: 128),
                        ClientUserId = c.String(nullable: false, maxLength: 128),
                        AssignedAt = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TrainerClientId)
                .ForeignKey("dbo.ClientProfiles", t => t.ClientUserId)
                .ForeignKey("dbo.TrainerProfiles", t => t.TrainerUserId)
                .Index(t => t.TrainerUserId)
                .Index(t => t.ClientUserId);
            
            CreateTable(
                "dbo.TrainerFeedbacks",
                c => new
                    {
                        TrainerFeedbackId = c.Int(nullable: false, identity: true),
                        TrainerUserId = c.String(nullable: false, maxLength: 128),
                        ClientUserId = c.String(nullable: false, maxLength: 128),
                        RelatedArea = c.String(maxLength: 100),
                        FeedbackText = c.String(nullable: false, maxLength: 1500),
                        Rating = c.Int(),
                        NextFocus = c.String(maxLength: 500),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TrainerFeedbackId)
                .ForeignKey("dbo.ClientProfiles", t => t.ClientUserId)
                .ForeignKey("dbo.TrainerProfiles", t => t.TrainerUserId)
                .Index(t => t.TrainerUserId)
                .Index(t => t.ClientUserId);
            
            CreateTable(
                "dbo.WorkoutExercises",
                c => new
                    {
                        WorkoutExerciseId = c.Int(nullable: false, identity: true),
                        WorkoutPlanId = c.Int(nullable: false),
                        ExerciseId = c.Int(nullable: false),
                        Sets = c.Int(nullable: false),
                        Repetitions = c.Int(nullable: false),
                        RestSeconds = c.Int(nullable: false),
                        DurationMinutes = c.Int(),
                        DisplayOrder = c.Int(nullable: false),
                        Notes = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.WorkoutExerciseId)
                .ForeignKey("dbo.Exercises", t => t.ExerciseId)
                .ForeignKey("dbo.WorkoutPlans", t => t.WorkoutPlanId)
                .Index(t => t.WorkoutPlanId)
                .Index(t => t.ExerciseId);
            
            CreateTable(
                "dbo.WorkoutPlans",
                c => new
                    {
                        WorkoutPlanId = c.Int(nullable: false, identity: true),
                        ClientUserId = c.String(nullable: false, maxLength: 128),
                        TrainerUserId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 150),
                        Description = c.String(maxLength: 1000),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.WorkoutPlanId)
                .ForeignKey("dbo.ClientProfiles", t => t.ClientUserId)
                .ForeignKey("dbo.TrainerProfiles", t => t.TrainerUserId)
                .Index(t => t.ClientUserId)
                .Index(t => t.TrainerUserId);
            
            CreateTable(
                "dbo.WorkoutLogs",
                c => new
                    {
                        WorkoutLogId = c.Int(nullable: false, identity: true),
                        ClientUserId = c.String(nullable: false, maxLength: 128),
                        WorkoutPlanId = c.Int(nullable: false),
                        StartedAt = c.DateTime(nullable: false),
                        CompletedAt = c.DateTime(),
                        DurationMinutes = c.Int(),
                        CaloriesBurned = c.Int(),
                        IsCompleted = c.Boolean(nullable: false),
                        Notes = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.WorkoutLogId)
                .ForeignKey("dbo.ClientProfiles", t => t.ClientUserId)
                .ForeignKey("dbo.WorkoutPlans", t => t.WorkoutPlanId)
                .Index(t => t.ClientUserId)
                .Index(t => t.WorkoutPlanId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkoutLogs", "WorkoutPlanId", "dbo.WorkoutPlans");
            DropForeignKey("dbo.WorkoutLogs", "ClientUserId", "dbo.ClientProfiles");
            DropForeignKey("dbo.WorkoutExercises", "WorkoutPlanId", "dbo.WorkoutPlans");
            DropForeignKey("dbo.WorkoutPlans", "TrainerUserId", "dbo.TrainerProfiles");
            DropForeignKey("dbo.WorkoutPlans", "ClientUserId", "dbo.ClientProfiles");
            DropForeignKey("dbo.WorkoutExercises", "ExerciseId", "dbo.Exercises");
            DropForeignKey("dbo.TrainerFeedbacks", "TrainerUserId", "dbo.TrainerProfiles");
            DropForeignKey("dbo.TrainerFeedbacks", "ClientUserId", "dbo.ClientProfiles");
            DropForeignKey("dbo.TrainerClients", "TrainerUserId", "dbo.TrainerProfiles");
            DropForeignKey("dbo.TrainerClients", "ClientUserId", "dbo.ClientProfiles");
            DropForeignKey("dbo.TrainerAvailabilities", "TrainerUserId", "dbo.TrainerProfiles");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Notifications", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.MealLogs", "DietPlanId", "dbo.DietPlans");
            DropForeignKey("dbo.MealLogs", "ClientUserId", "dbo.ClientProfiles");
            DropForeignKey("dbo.HealthMetrics", "ClientUserId", "dbo.ClientProfiles");
            DropForeignKey("dbo.FitnessGoals", "ClientUserId", "dbo.ClientProfiles");
            DropForeignKey("dbo.DietPlans", "TrainerUserId", "dbo.TrainerProfiles");
            DropForeignKey("dbo.Meals", "DietPlanId", "dbo.DietPlans");
            DropForeignKey("dbo.DietPlans", "ClientUserId", "dbo.ClientProfiles");
            DropForeignKey("dbo.AuditLogs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Appointments", "TrainerUserId", "dbo.TrainerProfiles");
            DropForeignKey("dbo.TrainerProfiles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Appointments", "ClientUserId", "dbo.ClientProfiles");
            DropForeignKey("dbo.ClientProfiles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.WorkoutLogs", new[] { "WorkoutPlanId" });
            DropIndex("dbo.WorkoutLogs", new[] { "ClientUserId" });
            DropIndex("dbo.WorkoutPlans", new[] { "TrainerUserId" });
            DropIndex("dbo.WorkoutPlans", new[] { "ClientUserId" });
            DropIndex("dbo.WorkoutExercises", new[] { "ExerciseId" });
            DropIndex("dbo.WorkoutExercises", new[] { "WorkoutPlanId" });
            DropIndex("dbo.TrainerFeedbacks", new[] { "ClientUserId" });
            DropIndex("dbo.TrainerFeedbacks", new[] { "TrainerUserId" });
            DropIndex("dbo.TrainerClients", new[] { "ClientUserId" });
            DropIndex("dbo.TrainerClients", new[] { "TrainerUserId" });
            DropIndex("dbo.TrainerAvailabilities", new[] { "TrainerUserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Notifications", new[] { "UserId" });
            DropIndex("dbo.MealLogs", new[] { "DietPlanId" });
            DropIndex("dbo.MealLogs", new[] { "ClientUserId" });
            DropIndex("dbo.HealthMetrics", new[] { "ClientUserId" });
            DropIndex("dbo.FitnessGoals", new[] { "ClientUserId" });
            DropIndex("dbo.Meals", new[] { "DietPlanId" });
            DropIndex("dbo.DietPlans", new[] { "TrainerUserId" });
            DropIndex("dbo.DietPlans", new[] { "ClientUserId" });
            DropIndex("dbo.AuditLogs", new[] { "UserId" });
            DropIndex("dbo.TrainerProfiles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.ClientProfiles", new[] { "UserId" });
            DropIndex("dbo.Appointments", new[] { "TrainerUserId" });
            DropIndex("dbo.Appointments", new[] { "ClientUserId" });
            DropTable("dbo.WorkoutLogs");
            DropTable("dbo.WorkoutPlans");
            DropTable("dbo.WorkoutExercises");
            DropTable("dbo.TrainerFeedbacks");
            DropTable("dbo.TrainerClients");
            DropTable("dbo.TrainerAvailabilities");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Notifications");
            DropTable("dbo.MealLogs");
            DropTable("dbo.HealthMetrics");
            DropTable("dbo.FitnessGoals");
            DropTable("dbo.Exercises");
            DropTable("dbo.Meals");
            DropTable("dbo.DietPlans");
            DropTable("dbo.AuditLogs");
            DropTable("dbo.TrainerProfiles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.ClientProfiles");
            DropTable("dbo.Appointments");
        }
    }
}
