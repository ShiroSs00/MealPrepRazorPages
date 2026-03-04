using Microsoft.EntityFrameworkCore;
using WebAppRazor.DAL.Data;
using WebAppRazor.DAL.Models;

namespace WebAppRazor.DAL.Repositories
{
    public class HealthProfileRepository : IHealthProfileRepository
    {
        private readonly AppDbContext _context;

        public HealthProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<HealthProfile?> GetLatestByUserIdAsync(int userId)
        {
            return await _context.HealthProfiles
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<List<HealthProfile>> GetAllByUserIdAsync(int userId)
        {
            return await _context.HealthProfiles
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(HealthProfile profile)
        {
            try
            {
                _context.HealthProfiles.Add(profile);
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
