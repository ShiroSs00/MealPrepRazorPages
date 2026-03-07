using WebAppRazor.BLL.DTOs;
using WebAppRazor.DAL.Models;
using WebAppRazor.DAL.Repositories;

namespace WebAppRazor.BLL.Services
{
    public class MealPlanService : IMealPlanService
    {
        private readonly IMealPlanRepository _mealPlanRepository;
        private readonly IHealthProfileRepository _healthProfileRepository;

        public MealPlanService(
            IMealPlanRepository mealPlanRepository,
            IHealthProfileRepository healthProfileRepository)
        {
            _mealPlanRepository = mealPlanRepository;
            _healthProfileRepository = healthProfileRepository;
        }

        public async Task<MealPlanDto> GenerateMenuAsync(int userId, double targetCalories, bool isPremium)
        {
            var plan = new MealPlan
            {
                UserId = userId,
                Title = $"Thực đơn ngày {DateTime.Now:dd/MM/yyyy}",
                TargetCalories = targetCalories,
                PlanDate = DateTime.Today,
                CreatedAt = DateTime.Now
            };

            // Use hardcoded menu logic instead of AI
            double breakfastCal = targetCalories * 0.25;
            double lunchCal = targetCalories * 0.35;
            double dinnerCal = targetCalories * 0.30;
            double snackCal = targetCalories * 0.10;

            plan.MealItems = GenerateFallbackMealItems(breakfastCal, lunchCal, dinnerCal, snackCal, isPremium);

            await _mealPlanRepository.CreateAsync(plan);
            return MapToDto(plan);
        }

        public async Task<MealPlanDto> CreateManualPlanAsync(int userId, List<MealItemDto> selectedItems)
        {
            var plan = new MealPlan
            {
                UserId = userId,
                Title = $"Thực đơn tự chọn {DateTime.Now:dd/MM/yyyy}",
                TargetCalories = selectedItems.Sum(i => i.Calories),
                PlanDate = DateTime.Today,
                CreatedAt = DateTime.Now
            };

            plan.MealItems = selectedItems.Select(item => new MealItem
            {
                MealType = item.MealType,
                Name = item.Name,
                Description = item.Description,
                Calories = item.Calories,
                Protein = item.Protein,
                Carbs = item.Carbs,
                Fat = item.Fat,
                Ingredients = item.Ingredients,
                CookingInstructions = item.CookingInstructions
            }).ToList();

            await _mealPlanRepository.CreateAsync(plan);
            return MapToDto(plan);
        }

        public async Task<bool> UpdateMealItemAsync(int mealItemId, MealItemDto updateDto)
        {
            var mealItem = await _mealPlanRepository.GetMealItemByIdAsync(mealItemId);
            if (mealItem == null) return false;

            mealItem.Name = updateDto.Name;
            mealItem.Description = updateDto.Description;
            mealItem.Calories = updateDto.Calories;
            mealItem.Protein = updateDto.Protein;
            mealItem.Carbs = updateDto.Carbs;
            mealItem.Fat = updateDto.Fat;
            mealItem.Ingredients = updateDto.Ingredients;
            mealItem.CookingInstructions = updateDto.CookingInstructions;

            // Assuming _mealPlanRepository has an UpdateMealItemAsync or similar
            // If not, we use the context directly or update through the repository
            return await _mealPlanRepository.UpdateMealItemAsync(mealItem);
        }

        public List<MealItemDto> GetAvailableMeals()
        {
            return MealLibrary.GetAllAvailableMeals();
        }

        private List<MealItem> GenerateFallbackMealItems(double breakfastCal, double lunchCal, double dinnerCal, double snackCal, bool isPremium)
        {
            var items = new List<MealItem>();
            var random = new Random();

            // Pick random meals from MealLibrary
            var breakfast = MealLibrary.BreakfastOptions[random.Next(MealLibrary.BreakfastOptions.Length)];
            var lunch = MealLibrary.LunchOptions[random.Next(MealLibrary.LunchOptions.Length)];
            var dinner = MealLibrary.DinnerOptions[random.Next(MealLibrary.DinnerOptions.Length)];
            var snack = MealLibrary.SnackOptions[random.Next(MealLibrary.SnackOptions.Length)];

            items.Add(CreateMealItem("Breakfast", breakfast, breakfastCal, isPremium));
            items.Add(CreateMealItem("Lunch", lunch, lunchCal, isPremium));
            items.Add(CreateMealItem("Dinner", dinner, dinnerCal, isPremium));
            items.Add(CreateMealItem("Snack", snack, snackCal, isPremium));

            return items;
        }

        private MealItem CreateMealItem(string mealType, (string name, string desc, double proteinRatio, double carbRatio, double fatRatio, string ingredients, string instructions) data, double calories, bool isPremium)
        {
            return new MealItem
            {
                MealType = mealType,
                Name = data.name,
                Description = data.desc,
                Calories = Math.Round(calories, 0),
                Protein = Math.Round(calories * data.proteinRatio / 4, 1), // 4 cal per gram protein
                Carbs = Math.Round(calories * data.carbRatio / 4, 1), // 4 cal per gram carbs
                Fat = Math.Round(calories * data.fatRatio / 9, 1), // 9 cal per gram fat
                Ingredients = isPremium ? data.ingredients : string.Empty,
                CookingInstructions = isPremium ? data.instructions : string.Empty
            };
        }

        public async Task<List<MealPlanDto>> GetUserPlansAsync(int userId)
        {
            var plans = await _mealPlanRepository.GetByUserIdAsync(userId);
            return plans.Select(MapToDto).ToList();
        }

        public async Task<MealPlanDto?> GetPlanWithItemsAsync(int planId)
        {
            var plan = await _mealPlanRepository.GetByIdWithItemsAsync(planId);
            return plan == null ? null : MapToDto(plan);
        }

        public async Task<bool> DeletePlanAsync(int planId)
        {
            return await _mealPlanRepository.DeleteAsync(planId);
        }

        private static MealPlanDto MapToDto(MealPlan entity)
        {
            return new MealPlanDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Title = entity.Title,
                TargetCalories = entity.TargetCalories,
                PlanDate = entity.PlanDate,
                CreatedAt = entity.CreatedAt,
                MealItems = entity.MealItems.Select(item => new MealItemDto
                {
                    Id = item.Id,
                    MealPlanId = item.MealPlanId,
                    MealType = item.MealType,
                    Name = item.Name,
                    Description = item.Description,
                    Calories = item.Calories,
                    Protein = item.Protein,
                    Carbs = item.Carbs,
                    Fat = item.Fat,
                    CookingInstructions = item.CookingInstructions,
                    Ingredients = item.Ingredients
                }).ToList()
            };
        }
    }
}
