using WebAppRazor.BLL.DTOs;

namespace WebAppRazor.BLL.Services
{
    public interface IReminderScheduleService
    {
        Task<List<ReminderScheduleDto>> GetUserSchedulesAsync(int userId);
        Task<bool> CreateScheduleAsync(int userId, string reminderType, TimeOnly reminderTime, DateOnly startDate, DateOnly? endDate, string repeatMode);
        Task<bool> DeleteScheduleAsync(int scheduleId, int userId);
        Task<bool> ToggleActiveAsync(int scheduleId, int userId);
        Task<List<ReminderScheduleDto>> GetAllActiveSchedulesAsync();
        Task<bool> UpdateLastTriggeredAsync(int scheduleId, DateTime triggeredAt);
    }
}
