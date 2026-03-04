using Microsoft.EntityFrameworkCore;
using WebAppRazor.DAL.Data;
using WebAppRazor.DAL.Models;

namespace WebAppRazor.DAL.Repositories
{
    public class ReminderScheduleRepository : IReminderScheduleRepository
    {
        private readonly AppDbContext _context;

        public ReminderScheduleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReminderSchedule>> GetByUserIdAsync(int userId)
        {
            return await _context.ReminderSchedules
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.ReminderTime)
                .ToListAsync();
        }

        public async Task<List<ReminderSchedule>> GetAllActiveAsync()
        {
            return await _context.ReminderSchedules
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        public async Task<ReminderSchedule?> GetByIdAsync(int id)
        {
            return await _context.ReminderSchedules.FindAsync(id);
        }

        public async Task<bool> CreateAsync(ReminderSchedule schedule)
        {
            try
            {
                _context.ReminderSchedules.Add(schedule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> UpdateAsync(ReminderSchedule schedule)
        {
            try
            {
                _context.ReminderSchedules.Update(schedule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var schedule = await _context.ReminderSchedules.FindAsync(id);
                if (schedule == null) return false;
                _context.ReminderSchedules.Remove(schedule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }
    }
}