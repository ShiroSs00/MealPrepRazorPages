using Microsoft.EntityFrameworkCore;
using WebAppRazor.DAL.Data;
using WebAppRazor.DAL.Models;

namespace WebAppRazor.DAL.Repositories
{
    public class MealReviewRepository : IMealReviewRepository
    {
        private readonly AppDbContext _context;

        public MealReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MealReview>> GetByMealItemIdAsync(int mealItemId)
        {
            return await _context.MealReviews
                .Where(r => r.MealItemId == mealItemId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<MealReview>> GetByUserIdAsync(int userId)
        {
            return await _context.MealReviews
                .Where(r => r.UserId == userId)
                .Include(r => r.MealItem)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<MealReview>> GetAllRecentAsync(int count = 20)
        {
            return await _context.MealReviews
                .Include(r => r.User)
                .Include(r => r.MealItem)
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<MealReview?> GetByIdAsync(int id)
        {
            return await _context.MealReviews
                .Include(r => r.User)
                .Include(r => r.MealItem)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> CreateAsync(MealReview review)
        {
            try
            {
                _context.MealReviews.Add(review);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<double> GetAverageRatingForMealItemAsync(int mealItemId)
        {
            var reviews = await _context.MealReviews
                .Where(r => r.MealItemId == mealItemId)
                .ToListAsync();

            if (reviews.Count == 0) return 0;
            return reviews.Average(r => r.Rating);
        }
    }
}
