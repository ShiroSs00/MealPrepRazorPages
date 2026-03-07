using WebAppRazor.BLL.DTOs;
using WebAppRazor.DAL.Models;
using WebAppRazor.DAL.Repositories;

namespace WebAppRazor.BLL.Services
{
    public class HealthProfileService : IHealthProfileService
    {
        private readonly IHealthProfileRepository _healthProfileRepository;

        public HealthProfileService(IHealthProfileRepository healthProfileRepository)
        {
            _healthProfileRepository = healthProfileRepository;
        }

        public HealthMetrics CalculateMetrics(int age, string gender, double heightCm, double weightKg, string activityLevel, string goal)
        {
            // BMI = weight(kg) / (height(m))^2
            double heightM = heightCm / 100.0;
            double bmi = weightKg / (heightM * heightM);

            // BMR (Mifflin-St Jeor)
            double bmr;
            if (gender.Equals("Male", StringComparison.OrdinalIgnoreCase))
            {
                bmr = 10 * weightKg + 6.25 * heightCm - 5 * age + 5;
            }
            else
            {
                bmr = 10 * weightKg + 6.25 * heightCm - 5 * age - 161;
            }

            // TDEE = BMR * activity multiplier
            double activityMultiplier = activityLevel switch
            {
                "Sedentary" => 1.2,
                "LightlyActive" => 1.375,
                "ModeratelyActive" => 1.55,
                "VeryActive" => 1.725,
                "ExtraActive" => 1.9,
                _ => 1.2
            };

            double tdee = bmr * activityMultiplier;

            // Daily calorie target based on goal
            double dailyCalorieTarget = goal switch
            {
                "LoseWeight" => tdee - 500,
                "GainWeight" => tdee + 500,
                _ => tdee // Maintain
            };

            // BMI Category
            string bmiCategory;
            if (bmi < 18.5) bmiCategory = "Thiếu cân";
            else if (bmi < 25) bmiCategory = "Bình thường";
            else if (bmi < 30) bmiCategory = "Thừa cân";
            else bmiCategory = "Béo phì";

            return new HealthMetrics
            {
                BMI = Math.Round(bmi, 1),
                BMR = Math.Round(bmr, 0),
                TDEE = Math.Round(tdee, 0),
                DailyCalorieTarget = Math.Round(dailyCalorieTarget, 0),
                BMICategory = bmiCategory
            };
        }

        public async Task<HealthProfileResult> SaveProfileAsync(int userId, int age, string gender, double heightCm, double weightKg, string activityLevel, string goal, string? allergies = null, string? favoriteFoods = null)
        {
            var metrics = CalculateMetrics(age, gender, heightCm, weightKg, activityLevel, goal);

            var profile = new HealthProfile
            {
                UserId = userId,
                Age = age,
                Gender = gender,
                Height = heightCm,
                Weight = weightKg,
                ActivityLevel = activityLevel,
                Goal = goal,
                BMI = metrics.BMI,
                BMR = metrics.BMR,
                TDEE = metrics.TDEE,
                DailyCalorieTarget = metrics.DailyCalorieTarget,
                Allergies = allergies,
                FavoriteFoods = favoriteFoods,
                CreatedAt = DateTime.Now
            };

            var success = await _healthProfileRepository.CreateAsync(profile);

            return new HealthProfileResult
            {
                Success = success,
                ErrorMessage = success ? null : "Không thể lưu hồ sơ sức khỏe. Vui lòng thử lại.",
                Metrics = metrics
            };
        }

        public async Task<HealthProfileDto?> GetLatestProfileAsync(int userId)
        {
            var profile = await _healthProfileRepository.GetLatestByUserIdAsync(userId);
            return profile == null ? null : MapToDto(profile);
        }

        public async Task<List<HealthProfileDto>> GetProfileHistoryAsync(int userId)
        {
            var profiles = await _healthProfileRepository.GetAllByUserIdAsync(userId);
            return profiles.Select(MapToDto).ToList();
        }

        private static HealthProfileDto MapToDto(HealthProfile entity)
        {
            return new HealthProfileDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Age = entity.Age,
                Gender = entity.Gender,
                Height = entity.Height,
                Weight = entity.Weight,
                ActivityLevel = entity.ActivityLevel,
                Goal = entity.Goal,
                BMI = entity.BMI,
                BMR = entity.BMR,
                TDEE = entity.TDEE,
                DailyCalorieTarget = entity.DailyCalorieTarget,
                Allergies = entity.Allergies,
                FavoriteFoods = entity.FavoriteFoods,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
