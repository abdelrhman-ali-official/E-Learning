namespace Shared.SubscriptionModels
{
    public class ValidateCouponDTO
    {
        public string CouponCode { get; set; } = string.Empty;
        public Guid PackageId { get; set; }
    }
}
