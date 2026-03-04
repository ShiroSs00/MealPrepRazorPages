namespace WebAppRazor.BLL.DTOs
{
    public class ProgressEntryDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double Weight { get; set; }
        public double BMI { get; set; }
        public double BMR { get; set; }
        public double TDEE { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime RecordedAt { get; set; }
    }
}
