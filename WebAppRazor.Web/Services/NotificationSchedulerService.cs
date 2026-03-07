using WebAppRazor.BLL.Services;
using WebAppRazor.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace WebAppRazor.Web.Services
{
    public class NotificationSchedulerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationSchedulerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                    var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

                    var dueNotifications = await notificationService.GetDueNotificationsAsync();

                    foreach (var n in dueNotifications)
                    {
                        await NotificationHub.SendNotificationToUser(
                            hubContext,
                            n.UserId,
                            n.Title,
                            n.Message,
                            n.Type);
                    }

                    if (dueNotifications.Count > 0)
                    {
                        var ids = dueNotifications.Select(n => n.Id).ToList();
                        await notificationService.MarkNotificationsAsSentAsync(ids);
                    }
                }
                catch
                {
                    // Tránh crash background service
                }

                // Poll mỗi 5 giây thay vì 30 giây → thông báo hiện gần như ngay lập tức
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}

