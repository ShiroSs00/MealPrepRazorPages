using WebAppRazor.BLL.DTOs;
using WebAppRazor.DAL.Models;
using WebAppRazor.DAL.Repositories;

namespace WebAppRazor.BLL.Services
{
    public class MealReviewService : IMealReviewService
    {
        private readonly IMealReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMealPlanRepository _mealPlanRepository;

        public MealReviewService(
            IMealReviewRepository reviewRepository,
            IUserRepository userRepository,
            IMealPlanRepository mealPlanRepository)
        {
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _mealPlanRepository = mealPlanRepository;
        }

        public async Task<ReviewResult> SubmitReviewAsync(int userId, int mealItemId, int rating, string comment)
        {
            int points = rating >= 4 ? 15 : 10; // Bonus points for high ratings

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new ReviewResult { Success = false, ErrorMessage = "Người dùng không tồn tại." };
            }

            var review = new MealReview
            {
                UserId = userId,
                MealItemId = mealItemId,
                Rating = rating,
                Comment = comment,
                PointsEarned = points,
                CreatedAt = DateTime.Now
            };

            var success = await _reviewRepository.CreateAsync(review);

            if (success)
            {
                // Update user review points
                user.ReviewPoints += points;
                await _userRepository.UpdateAsync(user);
            }

            // Populate names for SignalR DTO if success
            MealReviewDto? reviewDto = null;
            if (success)
             {
                 reviewDto = MapToDto(review);
                 reviewDto.UserFullName = user.FullName;
                 
                 // Get MealItem name
                 var mealItem = await _mealPlanRepository.GetMealItemByIdAsync(mealItemId);
                 reviewDto.MealItemName = mealItem?.Name;
             }

            return new ReviewResult
            {
                Success = success,
                ErrorMessage = success ? null : "Không thể gửi đánh giá. Vui lòng thử lại.",
                PointsEarned = success ? points : 0,
                Review = reviewDto
            };
        }

        public async Task<List<MealReviewDto>> GetReviewsByMealItemAsync(int mealItemId)
        {
            var reviews = await _reviewRepository.GetByMealItemIdAsync(mealItemId);
            return reviews.Select(MapToDto).ToList();
        }

        public async Task<List<MealReviewDto>> GetReviewsByUserAsync(int userId)
        {
            var reviews = await _reviewRepository.GetByUserIdAsync(userId);
            return reviews.Select(MapToDto).ToList();
        }

        public async Task<List<MealReviewDto>> GetRecentReviewsAsync(int count = 20)
        {
            var reviews = await _reviewRepository.GetAllRecentAsync(count);
            return reviews.Select(MapToDto).ToList();
        }

        public async Task<int> GetUserPointsAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.ReviewPoints ?? 0;
        }

        public async Task<ReviewResult> UpdateReviewAsync(int userId, int reviewId, int rating, string comment)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null || review.UserId != userId)
            {
                return new ReviewResult { Success = false, ErrorMessage = "Đánh giá không tồn tại hoặc bạn không có quyền sửa." };
            }

            review.Rating = rating;
            review.Comment = comment;
            
            var success = await _reviewRepository.UpdateAsync(review);
            
            MealReviewDto? reviewDto = null;
            if (success)
            {
                reviewDto = MapToDto(review);
                reviewDto.UserFullName = review.User?.FullName;
                reviewDto.MealItemName = review.MealItem?.Name;
            }

            return new ReviewResult
            {
                Success = success,
                ErrorMessage = success ? null : "Không thể cập nhật đánh giá.",
                Review = reviewDto
            };
        }

        public async Task<bool> DeleteReviewAsync(int userId, int reviewId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null || review.UserId != userId) return false;

            return await _reviewRepository.DeleteAsync(reviewId);
        }

        private static MealReviewDto MapToDto(MealReview entity)
        {
            return new MealReviewDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                MealItemId = entity.MealItemId,
                Rating = entity.Rating,
                Comment = entity.Comment,
                PointsEarned = entity.PointsEarned,
                CreatedAt = entity.CreatedAt,
                MealItemName = entity.MealItem?.Name,
                UserFullName = entity.User?.FullName
            };
        }
    }
}
