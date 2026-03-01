namespace Shared.SubscriptionModels
{
    public class CreatePackageDTO
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
        public List<PackageFeatureDTO> Features { get; set; } = new();
    }

    public class PackageFeatureDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
