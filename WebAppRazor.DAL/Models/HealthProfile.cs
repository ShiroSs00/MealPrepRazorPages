using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppRazor.DAL.Models
{
    public class HealthProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public int Age { get; set; }

        // "Male" or "Female"
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        // in cm
        public double Height { get; set; }

        // in kg
        public double Weight { get; set; }

        // "Sedentary", "LightlyActive", "ModeratelyActive", "VeryActive", "ExtraActive"
        [StringLength(30)]
        public string ActivityLevel { get; set; } = string.Empty;

        // "LoseWeight", "Maintain", "GainWeight"
        [StringLength(20)]
        public string Goal { get; set; } = string.Empty;

        // Calculated values
        public double BMI { get; set; }
        public double BMR { get; set; }
        public double TDEE { get; set; }

        // Daily calorie target based on goal
        public double DailyCalorieTarget { get; set; }

        [StringLength(200)]
        public string? Allergies { get; set; }

        [StringLength(200)]
        public string? FavoriteFoods { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
