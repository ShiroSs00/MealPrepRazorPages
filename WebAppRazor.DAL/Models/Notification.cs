using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppRazor.DAL.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        // "MealReminder", "ShoppingReminder", "ReviewReminder", "NewMenu", "Promotion", "System"
        [StringLength(30)]
        public string Type { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Thời điểm dự kiến gửi notification (UTC). Nếu null thì gửi ngay khi tạo.
        /// </summary>
        public DateTime? ScheduledAt { get; set; }

        /// <summary>
        /// Đã được push real-time qua SignalR chưa.
        /// </summary>
        public bool IsSent { get; set; } = true;
    }
}
