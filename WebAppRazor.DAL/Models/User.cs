using System.ComponentModel.DataAnnotations;

namespace WebAppRazor.DAL.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string SubscriptionTier { get; set; } = "Free";

        // PlanType: "Weekly" | "Monthly" | "Yearly" (null/empty when Free)
        [StringLength(10)]
        public string? SubscriptionPlanType { get; set; }

        public DateTime? SubscriptionExpiresAt { get; set; }

        public int ReviewPoints { get; set; } = 0;

        // Navigation properties
        public ICollection<HealthProfile> HealthProfiles { get; set; } = new List<HealthProfile>();
        public ICollection<MealPlan> MealPlans { get; set; } = new List<MealPlan>();
        public ICollection<MealReview> MealReviews { get; set; } = new List<MealReview>();
        public ICollection<ProgressEntry> ProgressEntries { get; set; } = new List<ProgressEntry>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<ReminderSchedule> ReminderSchedules { get; set; } = new List<ReminderSchedule>();
    }
}
