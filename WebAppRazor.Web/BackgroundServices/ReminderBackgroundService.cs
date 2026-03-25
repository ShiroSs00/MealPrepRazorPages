using Microsoft.AspNetCore.SignalR;
using WebAppRazor.BLL.Services;
using WebAppRazor.Web.Hubs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace WebAppRazor.Web.BackgroundServices
{
    public class ReminderBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<ReminderBackgroundService> _logger;

        public ReminderBackgroundService(
            IServiceProvider serviceProvider,
            IHubContext<NotificationHub> hubContext,
            ILogger<ReminderBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ReminderBackgroundService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessRemindersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing reminders.");
                }

                // Kiểm tra mỗi 10 giây để đảm bảo thông báo xuất hiện chính xác đúng giờ
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private readonly Dictionary<int, DateTime> _localLastTriggered = new();

        private async Task ProcessRemindersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var reminderScheduleService = scope.ServiceProvider.GetRequiredService<IReminderScheduleService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);
            var currentTime = TimeOnly.FromDateTime(now);

            var activeSchedules = await reminderScheduleService.GetAllActiveSchedulesAsync();

            foreach (var schedule in activeSchedules)
            {
                // Local memory deduplication block consecutive rapid-fires causing multi-toasts
                if (_localLastTriggered.TryGetValue(schedule.Id, out var lastLocal))
                {
                    if ((now - lastLocal).TotalMinutes < 2) continue;
                }

                // Kiểm tra ngày hợp lệ
                if (schedule.StartDate > today) continue;
                if (schedule.EndDate.HasValue && schedule.EndDate.Value < today) continue;

                // Kiểm tra chế độ lặp
                if (!ShouldTriggerToday(schedule.RepeatMode, now.DayOfWeek)) continue;

                // Kiểm tra đã đến giờ chưa (từ đúng thời điểm đó đến dưới 1 phút sau)
                var diffMinutes = (currentTime - schedule.ReminderTime).TotalMinutes;
                if (diffMinutes < 0 || diffMinutes >= 1) continue;

                // Tránh gửi trùng trong cùng 1 phút theo dữ liệu gốc DB
                if (schedule.LastTriggeredAt.HasValue &&
                    (now - schedule.LastTriggeredAt.Value).TotalMinutes < 2) continue;

                // Set local cache memory immediately to prevent polling overlapping
                _localLastTriggered[schedule.Id] = now;

                // Gửi thông báo
                await TriggerReminderAsync(notificationService, schedule);

                // Cập nhật thời gian gửi cuối qua BLL service
                await reminderScheduleService.UpdateLastTriggeredAsync(schedule.Id, now);

                _logger.LogInformation("Triggered reminder {Type} for user {UserId} at {Time}",
                    schedule.ReminderType, schedule.UserId, now);
            }
        }

        private static bool ShouldTriggerToday(string repeatMode, DayOfWeek dayOfWeek)
        {
            return repeatMode switch
            {
                "Daily" => true,
                "Weekdays" => dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday,
                "Weekends" => dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday,
                "Once" => true,
                _ => true
            };
        }

        private async Task TriggerReminderAsync(INotificationService notificationService, BLL.DTOs.ReminderScheduleDto schedule)
        {
            switch (schedule.ReminderType)
            {
                case "Breakfast":
                case "Lunch":
                case "Dinner":
                case "Snack":
                    await notificationService.CreateMealReminderAsync(schedule.UserId, schedule.ReminderType);
                    break;
                case "Shopping":
                    await notificationService.CreateShoppingReminderAsync(schedule.UserId);
                    break;
            }

            // Push real-time qua SignalR
            var (title, message) = schedule.ReminderType switch
            {
                "Breakfast" => ("🌅 Đến giờ ăn sáng!", "Hãy kiểm tra thực đơn bữa sáng của bạn."),
                "Lunch" => ("☀️ Đến giờ ăn trưa!", "Hãy kiểm tra thực đơn bữa trưa của bạn."),
                "Dinner" => ("🌙 Đến giờ ăn tối!", "Hãy kiểm tra thực đơn bữa tối của bạn."),
                "Shopping" => ("🛒 Nhắc mua nguyên liệu!", "Hãy kiểm tra danh sách nguyên liệu cần mua."),
                _ => ("🔔 Nhắc nhở!", "Bạn có một nhắc nhở mới.")
            };

            await NotificationHub.SendNotificationToUser(_hubContext, schedule.UserId, title, message, schedule.ReminderType + "Reminder");
        }
    }
}
