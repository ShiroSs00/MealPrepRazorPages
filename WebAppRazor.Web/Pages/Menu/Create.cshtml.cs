using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazor.BLL.DTOs;
using WebAppRazor.BLL.Services;

namespace WebAppRazor.Web.Pages.Menu
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IMealPlanService _mealPlanService;
        private readonly IHealthProfileService _healthProfileService;

        public CreateModel(IMealPlanService mealPlanService, IHealthProfileService healthProfileService)
        {
            _mealPlanService = mealPlanService;
            _healthProfileService = healthProfileService;
        }

        public List<MealItemDto> AvailableMeals { get; set; } = new();
        public HealthProfileDto? LatestProfile { get; set; }

        [BindProperty]
        public string SelectedBreakfastName { get; set; } = "";
        [BindProperty]
        public string SelectedLunchName { get; set; } = "";
        [BindProperty]
        public string SelectedDinnerName { get; set; } = "";
        [BindProperty]
        public string SelectedSnackName { get; set; } = "";

        public async Task OnGetAsync()
        {
            var userId = GetUserId();
            AvailableMeals = _mealPlanService.GetAvailableMeals();
            LatestProfile = await _healthProfileService.GetLatestProfileAsync(userId);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = GetUserId();
            var allMeals = _mealPlanService.GetAvailableMeals();
            var selectedItems = new List<MealItemDto>();
            
            void AddSelected(string type, string name) {
                var meal = allMeals.FirstOrDefault(m => m.Name == name && m.MealType == type);
                if (meal != null) {
                    selectedItems.Add(new MealItemDto {
                        MealType = type,
                        Name = meal.Name,
                        Description = meal.Description,
                        Calories = 500, // In real app, calculate based on target
                        Protein = 25,
                        Carbs = 60,
                        Fat = 15,
                        Ingredients = meal.Ingredients,
                        CookingInstructions = meal.CookingInstructions
                    });
                }
            }

            AddSelected("Breakfast", SelectedBreakfastName);
            AddSelected("Lunch", SelectedLunchName);
            AddSelected("Dinner", SelectedDinnerName);
            AddSelected("Snack", SelectedSnackName);

            if (selectedItems.Count > 0)
            {
                await _mealPlanService.CreateManualPlanAsync(userId, selectedItems);
            }

            return RedirectToPage("/Menu/Index");
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
