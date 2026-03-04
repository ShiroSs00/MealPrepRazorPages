using WebAppRazor.DAL.Repositories;

namespace WebAppRazor.BLL.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUserRepository _userRepository;

        public SubscriptionService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<SubscriptionResult> UpgradeToBasicPremiumAsync(int userId, string planType)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new SubscriptionResult
                {
                    Success = false,
                    ErrorMessage = "Không tìm thấy người dùng."
                };
            }

            DateTime expiresAt = planType switch
            {
                "Weekly" => DateTime.Now.AddDays(7),
                "Monthly" => DateTime.Now.AddMonths(1),
                "Yearly" => DateTime.Now.AddYears(1),
                _ => DateTime.Now.AddMonths(1)
            };

            user.SubscriptionTier = "BasicPremium";
            user.SubscriptionPlanType = planType is "Weekly" or "Monthly" or "Yearly" ? planType : "Monthly";
            user.SubscriptionExpiresAt = expiresAt;

            var success = await _userRepository.UpdateAsync(user);

            return new SubscriptionResult
            {
                Success = success,
                ErrorMessage = success ? null : "Không thể nâng cấp. Vui lòng thử lại.",
                Tier = "BasicPremium",
                ExpiresAt = expiresAt
            };
        }

        public async Task<string> GetUserTierAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return "Free";

            // Check if subscription expired
            if (user.SubscriptionTier == "BasicPremium" && user.SubscriptionExpiresAt.HasValue)
            {
                if (user.SubscriptionExpiresAt.Value < DateTime.Now)
                {
                    user.SubscriptionTier = "Free";
                    user.SubscriptionExpiresAt = null;
                    await _userRepository.UpdateAsync(user);
                    return "Free";
                }
            }

            return user.SubscriptionTier;
        }

        public async Task<bool> IsPremiumAsync(int userId)
        {
            var tier = await GetUserTierAsync(userId);
            return tier == "BasicPremium";
        }

        public async Task<SubscriptionDetailsResult> GetSubscriptionDetailsAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new SubscriptionDetailsResult();
            }

            // Check if subscription expired
            if (user.SubscriptionTier == "BasicPremium" && user.SubscriptionExpiresAt.HasValue)
            {
                if (user.SubscriptionExpiresAt.Value < DateTime.Now)
                {
                    user.SubscriptionTier = "Free";
                    user.SubscriptionPlanType = null;
                    user.SubscriptionExpiresAt = null;
                    await _userRepository.UpdateAsync(user);
                }
            }

            return new SubscriptionDetailsResult
            {
                Tier = user.SubscriptionTier,
                PlanType = user.SubscriptionPlanType,
                ExpiresAt = user.SubscriptionExpiresAt
            };
        }

        public async Task<SubscriptionResult> CancelSubscriptionAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new SubscriptionResult
                {
                    Success = false,
                    ErrorMessage = "Không tìm thấy người dùng."
                };
            }

            user.SubscriptionTier = "Free";
            user.SubscriptionPlanType = null;
            user.SubscriptionExpiresAt = null;

            var success = await _userRepository.UpdateAsync(user);

            return new SubscriptionResult
            {
                Success = success,
                ErrorMessage = success ? null : "Không thể hủy gói. Vui lòng thử lại.",
                Tier = user.SubscriptionTier,
                ExpiresAt = user.SubscriptionExpiresAt
            };
        }
    }
}
