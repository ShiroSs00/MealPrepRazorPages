using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppRazor.Web.Pages.Subscription
{
    [Authorize]
    public class PaymentMethodModel : PageModel
    {
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

        public void OnGet()
        {
            if (string.IsNullOrWhiteSpace(PlanType))
            {
                PlanType = "Monthly";
            }
        }

        public Task<IActionResult> OnPostCardAsync()
        {
            var redirect = RedirectToPage("Payment", new { planType = PlanType });
            return Task.FromResult<IActionResult>(redirect);
        }
    }
}

