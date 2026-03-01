using Shared.SubscriptionModels;

namespace Services.Abstractions
{
    public interface ICouponService
    {
        Task<CouponResponseDTO> CreateCouponAsync(CreateCouponDTO dto, CancellationToken cancellationToken = default);
        Task<CouponResponseDTO> GetCouponByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<IEnumerable<CouponResponseDTO>> GetAllCouponsAsync(CancellationToken cancellationToken = default);
        Task<bool> ValidateCouponAsync(string couponCode, CancellationToken cancellationToken = default);
        Task DeactivateCouponAsync(Guid couponId, CancellationToken cancellationToken = default);
        Task IncrementCouponUsageAsync(Guid couponId, CancellationToken cancellationToken = default);
    }
}
