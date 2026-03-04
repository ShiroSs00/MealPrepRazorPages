using Microsoft.EntityFrameworkCore;
using WebAppRazor.DAL.Data;
using WebAppRazor.DAL.Models;

namespace WebAppRazor.DAL.Repositories
{
    public class MealPlanRepository : IMealPlanRepository
    {
        private readonly AppDbContext _context;

        public MealPlanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MealPlan?> GetByIdAsync(int id)
        {
            return await _context.MealPlans.FindAsync(id);
        }

        public async Task<MealPlan?> GetByIdWithItemsAsync(int id)
        {
            return await _context.MealPlans
                .Include(m => m.MealItems)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<MealPlan>> GetByUserIdAsync(int userId)
        {
            return await _context.MealPlans
                .Where(m => m.UserId == userId)
                .Include(m => m.MealItems)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<MealPlan?> GetLatestByUserIdAsync(int userId)
        {
            return await _context.MealPlans
                .Where(m => m.UserId == userId)
                .Include(m => m.MealItems)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateAsync(MealPlan plan)
        {
            try
            {
                _context.MealPlans.Add(plan);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var plan = await _context.MealPlans.FindAsync(id);
                if (plan == null) return false;
                _context.MealPlans.Remove(plan);
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
