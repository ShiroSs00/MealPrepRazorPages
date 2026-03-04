using Microsoft.EntityFrameworkCore;
using WebAppRazor.DAL.Data;
using WebAppRazor.DAL.Models;

namespace WebAppRazor.DAL.Repositories
{
    public class ProgressRepository : IProgressRepository
    {
        private readonly AppDbContext _context;

        public ProgressRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProgressEntry>> GetByUserIdAsync(int userId)
        {
            return await _context.ProgressEntries
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.RecordedAt)
                .ToListAsync();
        }

        public async Task<ProgressEntry?> GetLatestByUserIdAsync(int userId)
        {
            return await _context.ProgressEntries
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.RecordedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateAsync(ProgressEntry entry)
        {
            try
            {
                _context.ProgressEntries.Add(entry);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
