namespace WebAppRazor.BLL.DTOs
{
    public class MealReviewDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MealItemId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int PointsEarned { get; set; }
        public DateTime CreatedAt { get; set; }

        // Flattened navigation data
        public string? MealItemName { get; set; }
        public string? UserFullName { get; set; }
    }
}
