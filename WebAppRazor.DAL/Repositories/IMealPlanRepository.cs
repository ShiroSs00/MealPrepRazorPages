using WebAppRazor.DAL.Models;

namespace WebAppRazor.DAL.Repositories
{
    public interface IMealPlanRepository
    {
        Task<MealPlan?> GetByIdAsync(int id);
        Task<MealPlan?> GetByIdWithItemsAsync(int id);
        Task<List<MealPlan>> GetByUserIdAsync(int userId);
        Task<MealPlan?> GetLatestByUserIdAsync(int userId);
        Task<bool> CreateAsync(MealPlan plan);
        Task<bool> DeleteAsync(int id);
    }
}
