using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazor.BLL.DTOs;
using WebAppRazor.BLL.Services;

namespace WebAppRazor.Web.Pages.Health
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IHealthProfileService _healthProfileService;

        public IndexModel(IHealthProfileService healthProfileService)
        {
            _healthProfileService = healthProfileService;
        }

        public HealthProfileDto? LatestProfile { get; set; }
        public List<HealthProfileDto> ProfileHistory { get; set; } = new();
        public HealthMetrics? Metrics { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            public int Age { get; set; } = 25;
            public string Gender { get; set; } = "Male";
            public double Height { get; set; } = 170;
            public double Weight { get; set; } = 65;
            public string ActivityLevel { get; set; } = "ModeratelyActive";
            public string Goal { get; set; } = "Maintain";
            public string? Allergies { get; set; }
            public string? FavoriteFoods { get; set; }
        }

        public async Task OnGetAsync()
        {
            var userId = GetUserId();
            LatestProfile = await _healthProfileService.GetLatestProfileAsync(userId);
            ProfileHistory = await _healthProfileService.GetProfileHistoryAsync(userId);

            if (LatestProfile != null)
            {
                Input.Age = LatestProfile.Age;
                Input.Gender = LatestProfile.Gender;
                Input.Height = LatestProfile.Height;
                Input.Weight = LatestProfile.Weight;
                Input.ActivityLevel = LatestProfile.ActivityLevel;
                Input.Goal = LatestProfile.Goal;
                Input.Allergies = LatestProfile.Allergies;
                Input.FavoriteFoods = LatestProfile.FavoriteFoods;

                Metrics = _healthProfileService.CalculateMetrics(
                    LatestProfile.Age, LatestProfile.Gender, LatestProfile.Height,
                    LatestProfile.Weight, LatestProfile.ActivityLevel, LatestProfile.Goal);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = GetUserId();

            var result = await _healthProfileService.SaveProfileAsync(
                userId, Input.Age, Input.Gender, Input.Height,
                Input.Weight, Input.ActivityLevel, Input.Goal,
                Input.Allergies, Input.FavoriteFoods);

            if (result.Success)
            {
                SuccessMessage = "Đã lưu hồ sơ sức khỏe thành công!";
                Metrics = result.Metrics;
            }
            else
            {
                ErrorMessage = result.ErrorMessage;
            }

            LatestProfile = await _healthProfileService.GetLatestProfileAsync(userId);
            ProfileHistory = await _healthProfileService.GetProfileHistoryAsync(userId);

            return Page();
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
