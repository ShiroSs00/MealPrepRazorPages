using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazor.BLL.Services;

namespace WebAppRazor.Web.Pages.Subscription
{
    [Authorize]
    public class PaymentModel : PageModel
    {
        private readonly ISubscriptionService _subscriptionService;

        public PaymentModel(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [BindProperty(SupportsGet = true)]
        public string PlanType { get; set; } = "Monthly";

        public string PlanTypeDisplay => PlanType switch
        {
            "Weekly" => "Gói Tuần",
            "Monthly" => "Gói Tháng",
            "Yearly" => "Gói Năm",
            _ => "Gói Tháng"
        };

        public string AmountDisplay => PlanType switch
        {
            "Weekly" => "39.000 đ",
            "Monthly" => "99.000 đ",
            "Yearly" => "799.000 đ",
            _ => "99.000 đ"
        };

        [BindProperty]
        public string BankName { get; set; } = string.Empty;

        [BindProperty]
        public string CardNumber { get; set; } = string.Empty;

        [BindProperty]
        public string CardHolder { get; set; } = string.Empty;

        [BindProperty]
        public string Expiry { get; set; } = string.Empty;

        [BindProperty]
        public string Otp { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            if (string.IsNullOrWhiteSpace(PlanType))
            {
                PlanType = "Monthly";
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(PlanType))
            {
                PlanType = "Monthly";
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Demo: chỉ cho thanh toán thành công nếu đúng thông tin thẻ sandbox VNPay
            var bankUpper = (BankName ?? string.Empty).Trim().ToUpperInvariant();
            var numberTrim = (CardNumber ?? string.Empty).Replace(" ", "").Trim();
            var holderUpper = (CardHolder ?? string.Empty).Trim().ToUpperInvariant();
            var expiryTrim = (Expiry ?? string.Empty).Trim();
            var otpTrim = (Otp ?? string.Empty).Trim();

            var isValidTestCard =
                bankUpper == "NCB" &&
                numberTrim == "9704198526191432198" &&
                holderUpper == "NGUYEN VAN A" &&
                expiryTrim == "07/15" &&
                otpTrim == "123456";

            if (!isValidTestCard)
            {
                ErrorMessage = "Thanh toán thất bại. Vui lòng nhập đúng thông tin thẻ test VNPay sandbox.";
                return Page();
            }

            var userId = GetUserId();
            var result = await _subscriptionService.UpgradeToBasicPremiumAsync(userId, PlanType);

            if (!result.Success)
            {
                ErrorMessage = result.ErrorMessage ?? "Thanh toán thất bại, vui lòng thử lại.";
                return Page();
            }

            TempData["SubscriptionMessage"] =
                $"Thanh toán thành công! Gói {PlanTypeDisplay} sẽ hết hạn ngày {result.ExpiresAt?.ToString("dd/MM/yyyy")}.";

            return RedirectToPage("/Subscription/Index");
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}

