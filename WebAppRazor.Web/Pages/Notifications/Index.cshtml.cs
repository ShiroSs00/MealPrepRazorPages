using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazor.BLL.DTOs;
using WebAppRazor.BLL.Services;

namespace WebAppRazor.Web.Pages.Notifications
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly INotificationService _notificationService;
        private readonly IReminderScheduleService _reminderScheduleService;

        public IndexModel(INotificationService notificationService, IReminderScheduleService reminderScheduleService)
        {
            _notificationService = notificationService;
            _reminderScheduleService = reminderScheduleService;
        }

        public List<NotificationDto> AllNotifications { get; set; } = new();
        public int UnreadCount { get; set; }

        public async Task OnGetAsync()
        {
            var userId = GetUserId();
            AllNotifications = await _notificationService.GetUserNotificationsAsync(userId);
            UnreadCount = await _notificationService.GetUnreadCountAsync(userId);
        }

        public async Task<IActionResult> OnPostMarkReadAsync(int notificationId)
        {
            await _notificationService.MarkAsReadAsync(notificationId);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostMarkAllReadAsync()
        {
            var userId = GetUserId();
            await _notificationService.MarkAllAsReadAsync(userId);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostScheduleMealReminderAsync(string mealType, DateTime reminderTime)
        {
            var userId = GetUserId();

            // reminderTime là giờ local từ người dùng, convert sang UTC để lưu
            var localTime = DateTime.SpecifyKind(reminderTime, DateTimeKind.Local);
            var utc = localTime.ToUniversalTime();

            await _notificationService.CreateScheduledMealReminderAsync(userId, mealType, utc);

            // FIX: Use TimeOnly.FromDateTime to extract TimeOnly from DateTime
            var startDate = DateOnly.FromDateTime(reminderTime);
            var time = TimeOnly.FromDateTime(reminderTime);

            // Define reminderType and repeatMode based on mealType (or use default values)
            var reminderType = mealType; // or set to a specific string if required
            var repeatMode = "Daily"; // or another appropriate default

            await _reminderScheduleService.CreateScheduleAsync(
                userId, reminderType, time, startDate, endDate: null, repeatMode);

            TempData["Success"] = $"Đã đặt lịch nhắc nhở thành công lúc {time:HH:mm} ngày {startDate:dd/MM/yyyy}!";
            return RedirectToPage();
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
