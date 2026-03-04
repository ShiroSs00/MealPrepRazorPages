using WebAppRazor.BLL.DTOs;

namespace WebAppRazor.BLL.Services
{
    public interface IMealPlanService
    {
        Task<MealPlanDto> GenerateMenuAsync(int userId, double targetCalories, bool isPremium);
        Task<List<MealPlanDto>> GetUserPlansAsync(int userId);
        Task<MealPlanDto?> GetPlanWithItemsAsync(int planId);
        Task<bool> DeletePlanAsync(int planId);
    }
}
