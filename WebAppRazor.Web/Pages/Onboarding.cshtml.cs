using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppRazor.Web.Pages
{
    [Authorize]
    public class OnboardingModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
