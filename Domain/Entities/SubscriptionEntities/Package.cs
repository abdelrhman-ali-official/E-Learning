namespace Domain.Entities.SubscriptionEntities
{
    public class Package : BaseEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PriceMonthly { get; set; }
        public decimal PriceYearly { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; }
        public int MaxCoursesAccess { get; set; }
        public bool HasLiveAccess { get; set; }
        public bool HasRecordedAccess { get; set; }
        public bool HasCertificate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<PackageFeature> Features { get; set; } = new List<PackageFeature>();
        public ICollection<StudentSubscription> Subscriptions { get; set; } = new List<StudentSubscription>();
    }
}
