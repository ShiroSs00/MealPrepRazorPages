namespace WebAppRazor.BLL.DTOs
{
    public class ReminderScheduleDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ReminderType { get; set; } = string.Empty;
        public TimeOnly ReminderTime { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string RepeatMode { get; set; } = "Daily";
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastTriggeredAt { get; set; }
    }
}
