using WebAppRazor.BLL.DTOs;

namespace WebAppRazor.BLL.Services
{
    public class ReviewResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public int PointsEarned { get; set; }
        public MealReviewDto? Review { get; set; }
    }

    public interface IMealReviewService
    {
        Task<ReviewResult> SubmitReviewAsync(int userId, int mealItemId, int rating, string comment);
        Task<List<MealReviewDto>> GetReviewsByMealItemAsync(int mealItemId);
        Task<List<MealReviewDto>> GetReviewsByUserAsync(int userId);
        Task<List<MealReviewDto>> GetRecentReviewsAsync(int count = 20);
        Task<int> GetUserPointsAsync(int userId);
        Task<ReviewResult> UpdateReviewAsync(int userId, int reviewId, int rating, string comment);
        Task<bool> DeleteReviewAsync(int userId, int reviewId);
    }
}
