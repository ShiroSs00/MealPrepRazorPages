using WebAppRazor.DAL.Models;

namespace WebAppRazor.DAL.Repositories
{
    public interface IReminderScheduleRepository
    {
        Task<List<ReminderSchedule>> GetByUserIdAsync(int userId);
        Task<List<ReminderSchedule>> GetAllActiveAsync();
        Task<ReminderSchedule?> GetByIdAsync(int id);
        Task<bool> CreateAsync(ReminderSchedule schedule);
        Task<bool> UpdateAsync(ReminderSchedule schedule);
        Task<bool> DeleteAsync(int id);
    }
}