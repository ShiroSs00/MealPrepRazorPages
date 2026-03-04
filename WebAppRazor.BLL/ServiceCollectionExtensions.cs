using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebAppRazor.BLL.Services;
using WebAppRazor.DAL.Data;
using WebAppRazor.DAL.Repositories;

namespace WebAppRazor.BLL.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure SQL Server Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Register DAL - Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IHealthProfileRepository, HealthProfileRepository>();
        services.AddScoped<IMealPlanRepository, MealPlanRepository>();
        services.AddScoped<IMealReviewRepository, MealReviewRepository>();
        services.AddScoped<IProgressRepository, ProgressRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IReminderScheduleRepository, ReminderScheduleRepository>();

        // Register BLL - Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IHealthProfileService, HealthProfileService>();
        services.AddScoped<IMealPlanService, MealPlanService>();
        services.AddScoped<IMealReviewService, MealReviewService>();
        services.AddScoped<IProgressService, ProgressService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IReminderScheduleService, ReminderScheduleService>();

        // Register OpenAI ChatGPT Service with HttpClient
        services.AddHttpClient<IAIService, OpenAIService>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Check if database already has Users table (existing DB)
        bool dbExists = false;
        try
        {
            var conn = db.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users'";
            var result = await cmd.ExecuteScalarAsync();
            dbExists = result != null && Convert.ToInt32(result) > 0;
            await conn.CloseAsync();
        }
        catch { /* DB does not exist yet */ }

        if (!dbExists)
        {
            // Fresh database - create all tables
            await db.Database.EnsureCreatedAsync();
        }
        else
        {
            // Existing database - add missing columns and tables via raw SQL
            var conn2 = db.Database.GetDbConnection();
            await conn2.OpenAsync();

            async Task ExecuteSqlAsync(string sql)
            {
                try
                {
                    using var c = conn2.CreateCommand();
                    c.CommandText = sql;
                    await c.ExecuteNonQueryAsync();
                }
                catch { /* Column/table already exists, skip */ }
            }

            // Add new columns to Users table
            await ExecuteSqlAsync("ALTER TABLE [Users] ADD [SubscriptionTier] nvarchar(20) NOT NULL DEFAULT 'Free'");
            await ExecuteSqlAsync("ALTER TABLE [Users] ADD [SubscriptionPlanType] nvarchar(10) NULL");
            await ExecuteSqlAsync("ALTER TABLE [Users] ADD [SubscriptionExpiresAt] datetime2 NULL");
            await ExecuteSqlAsync("ALTER TABLE [Users] ADD [ReviewPoints] int NOT NULL DEFAULT 0");

            // Create HealthProfiles table
            await ExecuteSqlAsync(@"CREATE TABLE [HealthProfiles] (
                [Id] int NOT NULL IDENTITY,
                [UserId] int NOT NULL,
                [Age] int NOT NULL,
                [Gender] nvarchar(10) NOT NULL DEFAULT '',
                [Height] float NOT NULL,
                [Weight] float NOT NULL,
                [ActivityLevel] nvarchar(30) NOT NULL DEFAULT '',
                [Goal] nvarchar(20) NOT NULL DEFAULT '',
                [BMI] float NOT NULL,
                [BMR] float NOT NULL,
                [TDEE] float NOT NULL,
                [DailyCalorieTarget] float NOT NULL,
                [CreatedAt] datetime2 NOT NULL,
                CONSTRAINT [PK_HealthProfiles] PRIMARY KEY ([Id]),
                CONSTRAINT [FK_HealthProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
            )");

            // Create MealPlans table
            await ExecuteSqlAsync(@"CREATE TABLE [MealPlans] (
                [Id] int NOT NULL IDENTITY,
                [UserId] int NOT NULL,
                [Title] nvarchar(200) NOT NULL DEFAULT '',
                [TargetCalories] float NOT NULL,
                [PlanDate] datetime2 NOT NULL,
                [CreatedAt] datetime2 NOT NULL,
                CONSTRAINT [PK_MealPlans] PRIMARY KEY ([Id]),
                CONSTRAINT [FK_MealPlans_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
            )");

            // Create MealItems table
            await ExecuteSqlAsync(@"CREATE TABLE [MealItems] (
                [Id] int NOT NULL IDENTITY,
                [MealPlanId] int NOT NULL,
                [MealType] nvarchar(20) NOT NULL DEFAULT '',
                [Name] nvarchar(200) NOT NULL DEFAULT '',
                [Description] nvarchar(max) NOT NULL DEFAULT '',
                [Calories] float NOT NULL,
                [Protein] float NOT NULL,
                [Carbs] float NOT NULL,
                [Fat] float NOT NULL,
                [Ingredients] nvarchar(max) NOT NULL DEFAULT '',
                [CookingInstructions] nvarchar(max) NOT NULL DEFAULT '',
                CONSTRAINT [PK_MealItems] PRIMARY KEY ([Id]),
                CONSTRAINT [FK_MealItems_MealPlans_MealPlanId] FOREIGN KEY ([MealPlanId]) REFERENCES [MealPlans] ([Id]) ON DELETE CASCADE
            )");

            // Create MealReviews table
            await ExecuteSqlAsync(@"CREATE TABLE [MealReviews] (
                [Id] int NOT NULL IDENTITY,
                [UserId] int NOT NULL,
                [MealItemId] int NOT NULL,
                [Rating] int NOT NULL,
                [Comment] nvarchar(max) NOT NULL DEFAULT '',
                [PointsEarned] int NOT NULL,
                [CreatedAt] datetime2 NOT NULL,
                CONSTRAINT [PK_MealReviews] PRIMARY KEY ([Id]),
                CONSTRAINT [FK_MealReviews_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
                CONSTRAINT [FK_MealReviews_MealItems_MealItemId] FOREIGN KEY ([MealItemId]) REFERENCES [MealItems] ([Id]) ON DELETE NO ACTION
            )");

            // Create ProgressEntries table
            await ExecuteSqlAsync(@"CREATE TABLE [ProgressEntries] (
                [Id] int NOT NULL IDENTITY,
                [UserId] int NOT NULL,
                [Weight] float NOT NULL,
                [BMI] float NOT NULL,
                [BMR] float NOT NULL,
                [TDEE] float NOT NULL,
                [Notes] nvarchar(max) NOT NULL DEFAULT '',
                [RecordedAt] datetime2 NOT NULL,
                CONSTRAINT [PK_ProgressEntries] PRIMARY KEY ([Id]),
                CONSTRAINT [FK_ProgressEntries_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
            )");

            // Create Notifications table
            await ExecuteSqlAsync(@"CREATE TABLE [Notifications] (
                [Id] int NOT NULL IDENTITY,
                [UserId] int NOT NULL,
                [Title] nvarchar(200) NOT NULL DEFAULT '',
                [Message] nvarchar(max) NOT NULL DEFAULT '',
                [Type] nvarchar(50) NOT NULL DEFAULT '',
                [IsRead] bit NOT NULL,
                [CreatedAt] datetime2 NOT NULL,
                [ScheduledAt] datetime2 NULL,
                [IsSent] bit NOT NULL DEFAULT 1,
                CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
                CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
            )");

            // Add new columns to Notifications table for existing DB
            await ExecuteSqlAsync("ALTER TABLE [Notifications] ADD [ScheduledAt] datetime2 NULL");
            await ExecuteSqlAsync("ALTER TABLE [Notifications] ADD [IsSent] bit NOT NULL DEFAULT 1");

            await conn2.CloseAsync();
        }
    }
}
