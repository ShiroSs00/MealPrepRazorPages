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
            LatestProfile = await _healthProfileService.GetLatestProfileAsync(userId);
            double targetCalories = LatestProfile?.DailyCalorieTarget ?? 2000;

            var selectedItems = new List<MealItemDto>();
            
            void AddSelected(string type, string name, double percentage) {
                (string name, string desc, double proteinRatio, double carbRatio, double fatRatio, string ingredients, string instructions) data = default;
                
                if (type == "Breakfast") data = MealLibrary.BreakfastOptions.FirstOrDefault(x => x.name == name);
                else if (type == "Lunch") data = MealLibrary.LunchOptions.FirstOrDefault(x => x.name == name);
                else if (type == "Dinner") data = MealLibrary.DinnerOptions.FirstOrDefault(x => x.name == name);
                else if (type == "Snack") data = MealLibrary.SnackOptions.FirstOrDefault(x => x.name == name);

                if (data.name != null) {
                    double cal = targetCalories * percentage;
                    selectedItems.Add(new MealItemDto {
                        MealType = type,
                        Name = data.name,
                        Description = data.desc,
                        Calories = Math.Round(cal, 0),
                        Protein = Math.Round(cal * data.proteinRatio / 4, 1),
                        Carbs = Math.Round(cal * data.carbRatio / 4, 1),
                        Fat = Math.Round(cal * data.fatRatio / 9, 1),
                        Ingredients = data.ingredients,
                        CookingInstructions = data.instructions
                    });
                }
            }

            AddSelected("Breakfast", SelectedBreakfastName, 0.25);
            AddSelected("Lunch", SelectedLunchName, 0.35);
            AddSelected("Dinner", SelectedDinnerName, 0.30);
            AddSelected("Snack", SelectedSnackName, 0.10);

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
