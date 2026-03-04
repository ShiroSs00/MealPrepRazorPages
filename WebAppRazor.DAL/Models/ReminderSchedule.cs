using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppRazor.DAL.Models
{
    public class ReminderSchedule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        // "Breakfast", "Lunch", "Dinner", "Shopping"
        [StringLength(30)]
        public string ReminderType { get; set; } = string.Empty;

        // Gi? nh?c (HH:mm) m?i ngày
        public TimeOnly ReminderTime { get; set; }

        // Ngày b?t ??u
        public DateOnly StartDate { get; set; }

        // Ngày k?t thúc (null = nh?c vô th?i h?n)
        public DateOnly? EndDate { get; set; }

        // L?p l?i: "Daily", "Weekdays", "Weekends", "Once"
        [StringLength(20)]
        public string RepeatMode { get; set; } = "Daily";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // L?n cu?i ?ã g?i thông báo (tránh g?i trùng)
        public DateTime? LastTriggeredAt { get; set; }
    }
}