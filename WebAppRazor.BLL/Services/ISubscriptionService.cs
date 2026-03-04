namespace WebAppRazor.BLL.Services
{
    public class SubscriptionResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string Tier { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
    }

    public class SubscriptionDetailsResult
    {
        public string Tier { get; set; } = "Free";
        public string? PlanType { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public interface ISubscriptionService
    {
        Task<SubscriptionResult> UpgradeToBasicPremiumAsync(int userId, string planType);
        Task<string> GetUserTierAsync(int userId);
        Task<bool> IsPremiumAsync(int userId);
        Task<SubscriptionResult> CancelSubscriptionAsync(int userId);
        Task<SubscriptionDetailsResult> GetSubscriptionDetailsAsync(int userId);
    }
}
