using WebAppRazor.DAL.Models;

namespace WebAppRazor.DAL.Repositories
{
    public interface IMealReviewRepository
    {
        Task<List<MealReview>> GetByMealItemIdAsync(int mealItemId);
        Task<List<MealReview>> GetByUserIdAsync(int userId);
        Task<List<MealReview>> GetAllRecentAsync(int count = 20);
        Task<MealReview?> GetByIdAsync(int id);
        Task<bool> CreateAsync(MealReview review);
        Task<double> GetAverageRatingForMealItemAsync(int mealItemId);
    }
}
