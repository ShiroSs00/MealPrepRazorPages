using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using WebAppRazor.BLL.DTOs;
using WebAppRazor.BLL.Services;
using WebAppRazor.Web.Hubs;

namespace WebAppRazor.Web.Pages.Reviews
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IMealReviewService _reviewService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public IndexModel(IMealReviewService reviewService, IHubContext<NotificationHub> hubContext)
        {
            _reviewService = reviewService;
            _hubContext = hubContext;
        }

        public List<MealReviewDto> MyReviews { get; set; } = new();
        public List<MealReviewDto> RecentReviews { get; set; } = new();
        public int UserPoints { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }
        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            var userId = GetUserId();
            MyReviews = await _reviewService.GetReviewsByUserAsync(userId);
            RecentReviews = await _reviewService.GetRecentReviewsAsync(20);

            UserPoints = await _reviewService.GetUserPointsAsync(userId);
        }

        public async Task<IActionResult> OnPostAsync(int mealItemId, int rating, string comment)
        {
            var userId = GetUserId();
            var result = await _reviewService.SubmitReviewAsync(userId, mealItemId, rating, comment);

            if (result.Success)
            {
                await NotificationHub.SendNotificationToUser(_hubContext, userId,
                    "Đánh giá thành công!", $"+{result.PointsEarned} điểm!", "System");

                // Real-time SignalR broadcast for other users
                if (result.Review != null)
                {
                    await NotificationHub.BroadcastReview(_hubContext, mealItemId, result.Review);
                }
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync(int reviewId, int rating, string comment)
        {
            var userId = GetUserId();
            var result = await _reviewService.UpdateReviewAsync(userId, reviewId, rating, comment);

            if (result.Success && result.Review != null)
            {
                await NotificationHub.BroadcastReviewUpdate(_hubContext, result.Review.MealItemId, result.Review);
                SuccessMessage = "Cập nhật đánh giá thành công!";
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Không thể cập nhật đánh giá.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int reviewId)
        {
            var userId = GetUserId();
            var success = await _reviewService.DeleteReviewAsync(userId, reviewId);

            if (success)
            {
                await NotificationHub.BroadcastReviewDelete(_hubContext, reviewId);
                SuccessMessage = "Đã xóa đánh giá.";
            }
            else
            {
                ErrorMessage = "Không thể xóa đánh giá.";
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
