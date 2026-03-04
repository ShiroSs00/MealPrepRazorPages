using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using WebAppRazor.BLL.Services;
using WebAppRazor.Web.Hubs;

namespace WebAppRazor.Web.Pages.Reviews
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IMealReviewService _reviewService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public CreateModel(
            IMealReviewService reviewService,
            INotificationService notificationService,
            IHubContext<NotificationHub> hubContext)
        {
            _reviewService = reviewService;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        [BindProperty]
        public int MealItemId { get; set; }

        [BindProperty]
        public int Rating { get; set; } = 5;

        [BindProperty]
        public string Comment { get; set; } = string.Empty;

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public void OnGet(int mealItemId)
        {
            MealItemId = mealItemId;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = GetUserId();

            var result = await _reviewService.SubmitReviewAsync(userId, MealItemId, Rating, Comment);

            if (result.Success)
            {
                // Send notification about points earned
                await _notificationService.CreateNotificationAsync(userId,
                    "Đánh giá thành công!",
                    $"Bạn đã nhận được {result.PointsEarned} điểm thưởng cho đánh giá!",
                    "System");
                await NotificationHub.SendNotificationToUser(_hubContext, userId,
                    "Đánh giá thành công!", $"+{result.PointsEarned} điểm!", "System");

                return RedirectToPage("/Reviews/Index");
            }

            ErrorMessage = result.ErrorMessage;
            return Page();
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
