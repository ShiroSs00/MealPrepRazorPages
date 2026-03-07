namespace WebAppRazor.BLL.Services
{
    public interface IAIService
    {
        Task<AIMenuResult> GenerateMenuWithAIAsync(double targetCalories, bool isPremium, string? goal = null, string? activityLevel = null, string? allergies = null, string? favoriteFoods = null);
    }

    public class AIMenuResult
    {
        public bool Success { get; set; }
        public List<AIMealItem> MealItems { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    public class AIMealItem
    {
        public string MealType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
        public string Ingredients { get; set; } = string.Empty;
        public string CookingInstructions { get; set; } = string.Empty;
    }
}
