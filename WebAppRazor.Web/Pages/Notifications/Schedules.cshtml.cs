using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazor.BLL.DTOs;
using WebAppRazor.BLL.Services;

namespace WebAppRazor.Web.Pages.Notifications
{
    [Authorize]
    public class SchedulesModel : PageModel
    {
        private readonly IReminderScheduleService _scheduleService;

        public SchedulesModel(IReminderScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        public List<ReminderScheduleDto> Schedules { get; set; } = new();

        [BindProperty]
        public string ReminderType { get; set; } = "Breakfast";

        [BindProperty]
        public TimeOnly ReminderTime { get; set; } = new TimeOnly(7, 0);

        [BindProperty]
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [BindProperty]
        public DateOnly? EndDate { get; set; }

        [BindProperty]
        public string RepeatMode { get; set; } = "Daily";

        public string? SuccessMessage { get; set; }

        public async Task OnGetAsync()
        {
            Schedules = await _scheduleService.GetUserSchedulesAsync(GetUserId());
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid) return Page();

            await _scheduleService.CreateScheduleAsync(
                GetUserId(), ReminderType, ReminderTime, StartDate, EndDate, RepeatMode);

            TempData["Success"] = "Đã tạo lịch nhắc nhở thành công!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int scheduleId)
        {
            await _scheduleService.DeleteScheduleAsync(scheduleId, GetUserId());
            TempData["Success"] = "Đã xóa lịch nhắc nhở.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostToggleAsync(int scheduleId)
        {
            await _scheduleService.ToggleActiveAsync(scheduleId, GetUserId());
            return RedirectToPage();
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}