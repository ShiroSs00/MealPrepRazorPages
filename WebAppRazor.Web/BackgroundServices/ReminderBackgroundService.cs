using Microsoft.AspNetCore.SignalR;
using WebAppRazor.BLL.Services;
using WebAppRazor.Web.Hubs;

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

            // Căn chỉnh lần đầu: đợi đến giây :00 gần nhất để poll đúng giờ
            var now = DateTime.Now;
            var delayToNextTick = TimeSpan.FromSeconds(10 - (now.Second % 10));
            await Task.Delay(delayToNextTick, stoppingToken);

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

                // Poll mỗi 10 giây → thông báo đến trong vòng 10 giây kể từ giờ đã set
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

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
                // Kiểm tra ngày hợp lệ
                if (schedule.StartDate > today) continue;
                if (schedule.EndDate.HasValue && schedule.EndDate.Value < today) continue;

                // Kiểm tra chế độ lặp
                if (!ShouldTriggerToday(schedule.RepeatMode, now.DayOfWeek)) continue;

                // Kiểm tra đã đến giờ chưa (window ±30 giây để không bỏ sót khi poll mỗi 10s)
                var diffSeconds = Math.Abs((currentTime - schedule.ReminderTime).TotalSeconds);
                if (diffSeconds > 30) continue;

                // Tránh gửi trùng trong vòng 2 phút
                if (schedule.LastTriggeredAt.HasValue &&
                    (now - schedule.LastTriggeredAt.Value).TotalMinutes < 2) continue;

                // Push SignalR ngay lập tức
                await TriggerReminderAsync(notificationService, schedule);

                // Cập nhật thời gian gửi cuối
                await reminderScheduleService.UpdateLastTriggeredAsync(schedule.Id, now);

                _logger.LogInformation("Triggered reminder {Type} for user {UserId} at {Time}",
                    schedule.ReminderType, schedule.UserId, now);
            }
        }

        private static bool ShouldTriggerToday(string repeatMode, DayOfWeek dayOfWeek)
        {
            return repeatMode switch
            {
                "Daily"    => true,
                "Weekdays" => dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday,
                "Weekends" => dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday,
                "Once"     => true,
                _          => true
            };
        }

        private async Task TriggerReminderAsync(INotificationService notificationService, BLL.DTOs.ReminderScheduleDto schedule)
        {
            // Lưu notification vào DB
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

            // Push SignalR ngay lập tức đến client
            var (title, message) = schedule.ReminderType switch
            {
                "Breakfast" => ("🌅 Đến giờ ăn sáng!", "Hãy kiểm tra thực đơn bữa sáng của bạn."),
                "Lunch"     => ("☀️ Đến giờ ăn trưa!", "Hãy kiểm tra thực đơn bữa trưa của bạn."),
                "Dinner"    => ("🌙 Đến giờ ăn tối!", "Hãy kiểm tra thực đơn bữa tối của bạn."),
                "Shopping"  => ("🛒 Nhắc mua nguyên liệu!", "Hãy kiểm tra danh sách nguyên liệu cần mua."),
                _           => ("🔔 Nhắc nhở!", "Bạn có một nhắc nhở mới.")
            };

            await NotificationHub.SendNotificationToUser(
                _hubContext,
                schedule.UserId,
                title,
                message,
                schedule.ReminderType + "Reminder");
        }
    }
}
