using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using WebAppRazor.BLL.DTOs;
using WebAppRazor.BLL.Services;
using WebAppRazor.Web.Hubs;

namespace WebAppRazor.Web.Pages.Menu
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IMealPlanService _mealPlanService;
        private readonly IHealthProfileService _healthProfileService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public IndexModel(
            IMealPlanService mealPlanService,
            IHealthProfileService healthProfileService,
            ISubscriptionService subscriptionService,
            INotificationService notificationService,
            IHubContext<NotificationHub> hubContext)
        {
            _mealPlanService = mealPlanService;
            _healthProfileService = healthProfileService;
            _subscriptionService = subscriptionService;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        public List<MealPlanDto> MealPlans { get; set; } = new();
        public HealthProfileDto? LatestProfile { get; set; }
        public bool IsPremium { get; set; }
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            var userId = GetUserId();
            MealPlans = await _mealPlanService.GetUserPlansAsync(userId);
            LatestProfile = await _healthProfileService.GetLatestProfileAsync(userId);
            IsPremium = await _subscriptionService.IsPremiumAsync(userId);
        }

        public async Task<IActionResult> OnPostGenerateAsync()
        {
            var userId = GetUserId();
            LatestProfile = await _healthProfileService.GetLatestProfileAsync(userId);

            if (LatestProfile == null)
            {
                ErrorMessage = "Vui lòng cập nhật chỉ số sức khỏe trước khi tạo thực đơn.";
                MealPlans = await _mealPlanService.GetUserPlansAsync(userId);
                IsPremium = await _subscriptionService.IsPremiumAsync(userId);
                return Page();
            }

            IsPremium = await _subscriptionService.IsPremiumAsync(userId);
            try
            {
                var plan = await _mealPlanService.GenerateMenuAsync(userId, LatestProfile.DailyCalorieTarget, IsPremium);

                // Send notification
                await _notificationService.CreateNewMenuNotificationAsync(userId);
                await NotificationHub.SendNotificationToUser(_hubContext, userId,
                    "Thực đơn mới!", "Thực đơn cá nhân hóa của bạn đã sẵn sàng!", "NewMenu");

                SuccessMessage = "Đã tạo thực đơn mới thành công!";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            MealPlans = await _mealPlanService.GetUserPlansAsync(userId);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int planId)
        {
            await _mealPlanService.DeletePlanAsync(planId);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditMealAsync(int mealItemId, MealItemDto updateDto)
        {
            var success = await _mealPlanService.UpdateMealItemAsync(mealItemId, updateDto);
            if (success)
            {
                SuccessMessage = "Đã cập nhật món ăn thành công!";
            }
            else
            {
                ErrorMessage = "Không thể cập nhật món ăn.";
            }
            return RedirectToPage();
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
