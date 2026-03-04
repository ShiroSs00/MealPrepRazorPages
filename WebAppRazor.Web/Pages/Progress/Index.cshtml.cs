using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazor.BLL.DTOs;
using WebAppRazor.BLL.Services;

namespace WebAppRazor.Web.Pages.Progress
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IProgressService _progressService;
        private readonly IHealthProfileService _healthProfileService;

        public IndexModel(IProgressService progressService, IHealthProfileService healthProfileService)
        {
            _progressService = progressService;
            _healthProfileService = healthProfileService;
        }

        public List<ProgressEntryDto> ProgressHistory { get; set; } = new();
        public HealthProfileDto? LatestProfile { get; set; }

        [BindProperty]
        public double NewWeight { get; set; }

        [BindProperty]
        public string Notes { get; set; } = string.Empty;

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        // Chart data
        public string ChartLabels { get; set; } = "[]";
        public string ChartWeights { get; set; } = "[]";
        public string ChartBMIs { get; set; } = "[]";
        public string ChartTDEEs { get; set; } = "[]";

        public async Task OnGetAsync()
        {
            var userId = GetUserId();
            await LoadDataAsync(userId);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = GetUserId();

            var success = await _progressService.LogProgressAsync(userId, NewWeight, Notes);

            if (success)
            {
                SuccessMessage = "Đã ghi nhận tiến trình thành công!";
            }
            else
            {
                ErrorMessage = "Không thể ghi nhận. Vui lòng thử lại.";
            }

            await LoadDataAsync(userId);
            return Page();
        }

        private async Task LoadDataAsync(int userId)
        {
            ProgressHistory = await _progressService.GetProgressHistoryAsync(userId);
            LatestProfile = await _healthProfileService.GetLatestProfileAsync(userId);

            if (LatestProfile != null)
            {
                NewWeight = LatestProfile.Weight;
            }

            // Prepare chart data
            if (ProgressHistory.Count > 0)
            {
                var labels = ProgressHistory.Select(p => p.RecordedAt.ToString("dd/MM")).ToList();
                var weights = ProgressHistory.Select(p => p.Weight).ToList();
                var bmis = ProgressHistory.Select(p => p.BMI).ToList();
                var tdees = ProgressHistory.Select(p => p.TDEE).ToList();

                ChartLabels = System.Text.Json.JsonSerializer.Serialize(labels);
                ChartWeights = System.Text.Json.JsonSerializer.Serialize(weights);
                ChartBMIs = System.Text.Json.JsonSerializer.Serialize(bmis);
                ChartTDEEs = System.Text.Json.JsonSerializer.Serialize(tdees);
            }
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
