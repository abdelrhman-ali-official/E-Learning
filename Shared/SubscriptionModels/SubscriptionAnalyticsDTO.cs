namespace Shared.SubscriptionModels
{
    public class SubscriptionAnalyticsDTO
    {
        public int TotalActiveSubscriptions { get; set; }
        public int TotalPendingPayments { get; set; }
        public int TotalExpiredSubscriptions { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal YearlyRevenue { get; set; }
        public List<PackageStatsDTO> PackageStats { get; set; } = new();
    }

    public class PackageStatsDTO
    {
        public Guid PackageId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public int SubscriptionCount { get; set; }
        public decimal Revenue { get; set; }
    }
}
