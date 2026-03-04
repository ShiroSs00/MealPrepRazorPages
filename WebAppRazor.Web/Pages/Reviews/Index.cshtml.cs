using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazor.BLL.DTOs;
using WebAppRazor.BLL.Services;

namespace WebAppRazor.Web.Pages.Reviews
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IMealReviewService _reviewService;

        public IndexModel(IMealReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public List<MealReviewDto> MyReviews { get; set; } = new();
        public List<MealReviewDto> RecentReviews { get; set; } = new();
        public int UserPoints { get; set; }

        public async Task OnGetAsync()
        {
            var userId = GetUserId();
            MyReviews = await _reviewService.GetReviewsByUserAsync(userId);
            RecentReviews = await _reviewService.GetRecentReviewsAsync(20);

            UserPoints = await _reviewService.GetUserPointsAsync(userId);
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
