namespace WebAppRazor.BLL.DTOs
{
    public class MealPlanDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public double TargetCalories { get; set; }
        public DateTime PlanDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<MealItemDto> MealItems { get; set; } = new();
    }

    public class MealItemDto
    {
        public int Id { get; set; }
        public int MealPlanId { get; set; }
        public string MealType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
        public string CookingInstructions { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
    }
}
