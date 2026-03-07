namespace WebAppRazor.BLL.DTOs
{
    public class HealthProfileDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public double Height { get; set; }
        public double Weight { get; set; }
        public string ActivityLevel { get; set; } = string.Empty;
        public string Goal { get; set; } = string.Empty;
        public double BMI { get; set; }
        public double BMR { get; set; }
        public double TDEE { get; set; }
        public double DailyCalorieTarget { get; set; }
        public string? Allergies { get; set; }
        public string? FavoriteFoods { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
