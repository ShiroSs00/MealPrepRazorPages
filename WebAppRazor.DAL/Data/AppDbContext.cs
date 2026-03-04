using Microsoft.EntityFrameworkCore;
using WebAppRazor.DAL.Models;

namespace WebAppRazor.DAL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<HealthProfile> HealthProfiles { get; set; } = null!;
        public DbSet<MealPlan> MealPlans { get; set; } = null!;
        public DbSet<MealItem> MealItems { get; set; } = null!;
        public DbSet<MealReview> MealReviews { get; set; } = null!;
        public DbSet<ProgressEntry> ProgressEntries { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<ReminderSchedule> ReminderSchedules { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<HealthProfile>()
                .HasOne(h => h.User)
                .WithMany(u => u.HealthProfiles)
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MealPlan>()
                .HasOne(m => m.User)
                .WithMany(u => u.MealPlans)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MealItem>()
                .HasOne(mi => mi.MealPlan)
                .WithMany(mp => mp.MealItems)
                .HasForeignKey(mi => mi.MealPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MealReview>()
                .HasOne(r => r.User)
                .WithMany(u => u.MealReviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MealReview>()
                .HasOne(r => r.MealItem)
                .WithMany(mi => mi.Reviews)
                .HasForeignKey(r => r.MealItemId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProgressEntry>()
                .HasOne(p => p.User)
                .WithMany(u => u.ProgressEntries)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReminderSchedule>()
                .HasOne(r => r.User)
                .WithMany(u => u.ReminderSchedules)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
