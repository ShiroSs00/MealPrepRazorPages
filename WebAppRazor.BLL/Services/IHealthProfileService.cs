using WebAppRazor.BLL.DTOs;

namespace WebAppRazor.BLL.Services
{
    public class HealthMetrics
    {
        public double BMI { get; set; }
        public double BMR { get; set; }
        public double TDEE { get; set; }
        public double DailyCalorieTarget { get; set; }
        public string BMICategory { get; set; } = string.Empty;
    }

    public class HealthProfileResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public HealthMetrics? Metrics { get; set; }
    }

    public interface IHealthProfileService
    {
        HealthMetrics CalculateMetrics(int age, string gender, double heightCm, double weightKg, string activityLevel, string goal);
        Task<HealthProfileResult> SaveProfileAsync(int userId, int age, string gender, double heightCm, double weightKg, string activityLevel, string goal, string? allergies = null, string? favoriteFoods = null);
        Task<HealthProfileDto?> GetLatestProfileAsync(int userId);
        Task<List<HealthProfileDto>> GetProfileHistoryAsync(int userId);
    }
}
