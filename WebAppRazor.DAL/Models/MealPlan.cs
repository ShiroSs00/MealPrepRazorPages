using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppRazor.DAL.Models
{
    public class MealPlan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        // Target calories for this plan
        public double TargetCalories { get; set; }

        public DateTime PlanDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<MealItem> MealItems { get; set; } = new List<MealItem>();
    }

    public class MealItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MealPlanId { get; set; }

        [ForeignKey("MealPlanId")]
        public MealPlan MealPlan { get; set; } = null!;

        // "Breakfast", "Lunch", "Dinner", "Snack"
        [StringLength(20)]
        public string MealType { get; set; } = string.Empty;

        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }

        // Premium only: cooking instructions
        [StringLength(5000)]
        public string CookingInstructions { get; set; } = string.Empty;

        // Premium only: detailed ingredients
        [StringLength(2000)]
        public string Ingredients { get; set; } = string.Empty;

        // Track completion status
        public bool IsCompleted { get; set; } = false;

        public ICollection<MealReview> Reviews { get; set; } = new List<MealReview>();
    }
}
