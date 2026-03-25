using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazor.BLL.DTOs;
using WebAppRazor.BLL.Interfaces;
using WebAppRazor.BLL.Services;

namespace WebAppRazor.Web.Pages.Progress
{
    [Authorize]
    public class StatisticsModel : PageModel
    {
        private readonly IDashboardService _dashboardService;

        public StatisticsModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [BindProperty(SupportsGet = true)]
        public string FilterType { get; set; } = "Month"; // Default to Month

        public DashboardDto Dashboard { get; set; } = new DashboardDto();

        // JSON properties for Chart.js
        public string ChartLabelsWeightHistory { get; set; } = "[]";
        public string ChartDataWeightHistory { get; set; } = "[]";

        public string ChartLabelsCalories7Days { get; set; } = "[]";
        public string ChartDataCalories7Days { get; set; } = "[]";

        public string ChartDataNutrition { get; set; } = "[]";

        public string ChartLabelsProgress { get; set; } = "[]";
        public string ChartDataProgress { get; set; } = "[]";

        public async Task OnGetAsync()
        {
            var userId = GetUserId();
            Dashboard = await _dashboardService.GetDashboardDataAsync(userId, FilterType);

            // Serialize data for charts
            if (Dashboard.WeightHistory.Count > 0)
            {
                ChartLabelsWeightHistory = System.Text.Json.JsonSerializer.Serialize(Dashboard.WeightHistory.Select(x => x.Label));
                ChartDataWeightHistory = System.Text.Json.JsonSerializer.Serialize(Dashboard.WeightHistory.Select(x => x.Value));
            }

            if (Dashboard.CaloriesLast7Days.Count > 0)
            {
                ChartLabelsCalories7Days = System.Text.Json.JsonSerializer.Serialize(Dashboard.CaloriesLast7Days.Select(x => x.Label));
                ChartDataCalories7Days = System.Text.Json.JsonSerializer.Serialize(Dashboard.CaloriesLast7Days.Select(x => x.Value));
            }

            // Nutrition Donut
            var nutritionData = new[] { Dashboard.ProteinToday, Dashboard.CarbsToday, Dashboard.FatToday };
            ChartDataNutrition = System.Text.Json.JsonSerializer.Serialize(nutritionData);

            // Progress line chart
            if (Dashboard.ProgressWeight.Count > 0)
            {
                ChartLabelsProgress = System.Text.Json.JsonSerializer.Serialize(Dashboard.ProgressWeight.Select(x => x.Label));
                ChartDataProgress = System.Text.Json.JsonSerializer.Serialize(Dashboard.ProgressWeight.Select(x => x.Value));
            }
        }

        public async Task<IActionResult> OnPostSeedDataAsync()
        {
            var userId = GetUserId();
            await _dashboardService.SeedMockDataAsync(userId);
            return RedirectToPage();
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
