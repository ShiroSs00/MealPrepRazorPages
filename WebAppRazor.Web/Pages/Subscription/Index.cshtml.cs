using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazor.BLL.Services;

namespace WebAppRazor.Web.Pages.Subscription
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ISubscriptionService _subscriptionService;

        public IndexModel(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public string CurrentTier { get; set; } = "Free";
        public string? CurrentPlanType { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public string CurrentPlanDisplay => CurrentPlanType switch
        {
            "Weekly" => "Gói Tuần - 39K đ",
            "Monthly" => "Gói Tháng - 99K đ",
            "Yearly" => "Gói Năm - 799K đ",
            _ => "Gói Tháng - 99K đ"
        };

        public string CurrentPlanPriceShort => CurrentPlanType switch
        {
            "Weekly" => "39K đ",
            "Monthly" => "99K đ",
            "Yearly" => "799K đ",
            _ => "99K đ"
        };

        public string CurrentPlanPeriodShort => CurrentPlanType switch
        {
            "Weekly" => "/tuần",
            "Monthly" => "/tháng",
            "Yearly" => "/năm",
            _ => "/tháng"
        };

        public async Task OnGetAsync()
        {
            var userId = GetUserId();
            var details = await _subscriptionService.GetSubscriptionDetailsAsync(userId);
            CurrentTier = details.Tier;
            ExpiresAt = details.ExpiresAt;
            CurrentPlanType = details.PlanType;

            if (TempData["SubscriptionMessage"] is string msg)
            {
                SuccessMessage = msg;
            }
        }

        public Task<IActionResult> OnPostUpgradeAsync(string planType)
        {
            // Bước 1: chọn gói -> sang bước chọn phương thức thanh toán
            var redirect = RedirectToPage("PaymentMethod", new { planType });
            return Task.FromResult<IActionResult>(redirect);
        }

        public async Task<IActionResult> OnPostCancelAsync()
        {
            var userId = GetUserId();
            var result = await _subscriptionService.CancelSubscriptionAsync(userId);

            if (result.Success)
            {
                SuccessMessage = "Bạn đã hủy gói Basic Premium. Tài khoản quay về gói Miễn phí.";
            }
            else
            {
                ErrorMessage = result.ErrorMessage;
            }

            await OnGetAsync();
            return Page();
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
