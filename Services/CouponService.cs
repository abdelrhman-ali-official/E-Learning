using AutoMapper;
using Domain.Contracts;
using Domain.Entities.SubscriptionEntities;
using Domain.Exceptions;
using Services.Abstractions;
using Shared.SubscriptionModels;

namespace Services
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;

        public CouponService(IUnitOFWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CouponResponseDTO> CreateCouponAsync(CreateCouponDTO dto, CancellationToken cancellationToken = default)
        {
            var coupons = await _unitOfWork.GetRepository<DiscountCoupon, Guid>().GetAllAsync();
            if (coupons.Any(c => c.Code == dto.Code))
                throw new ValidationException(new[] { $"A coupon with code '{dto.Code}' already exists" });

            var coupon = _mapper.Map<DiscountCoupon>(dto);
            coupon.UsedCount = 0;
            coupon.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.GetRepository<DiscountCoupon, Guid>().AddAsync(coupon);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CouponResponseDTO>(coupon);
        }

        public async Task<CouponResponseDTO> GetCouponByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            var coupons = await _unitOfWork.GetRepository<DiscountCoupon, Guid>().GetAllAsync();
            var coupon = coupons.FirstOrDefault(c => c.Code == code);

            if (coupon == null)
                throw new CouponNotFoundException(code);

            return _mapper.Map<CouponResponseDTO>(coupon);
        }

        public async Task<IEnumerable<CouponResponseDTO>> GetAllCouponsAsync(CancellationToken cancellationToken = default)
        {
            var coupons = await _unitOfWork.GetRepository<DiscountCoupon, Guid>().GetAllAsync();
            return _mapper.Map<IEnumerable<CouponResponseDTO>>(coupons);
        }

        public async Task<bool> ValidateCouponAsync(string couponCode, CancellationToken cancellationToken = default)
        {
            var coupons = await _unitOfWork.GetRepository<DiscountCoupon, Guid>().GetAllAsync();
            var coupon = coupons.FirstOrDefault(c => c.Code == couponCode);

            if (coupon == null)
                return false;

            if (!coupon.IsActive)
                return false;

            if (coupon.ExpiryDate < DateTime.UtcNow)
                return false;

            if (coupon.UsedCount >= coupon.MaxUsage)
                return false;

            return true;
        }

        public async Task DeactivateCouponAsync(Guid couponId, CancellationToken cancellationToken = default)
        {
            var coupon = await _unitOfWork.GetRepository<DiscountCoupon, Guid>().GetAsync(couponId);
            if (coupon == null)
                throw new CouponNotFoundException(couponId);

            coupon.IsActive = false;
            _unitOfWork.GetRepository<DiscountCoupon, Guid>().Update(coupon);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task IncrementCouponUsageAsync(Guid couponId, CancellationToken cancellationToken = default)
        {
            var coupon = await _unitOfWork.GetRepository<DiscountCoupon, Guid>().GetAsync(couponId);
            if (coupon == null)
                throw new CouponNotFoundException(couponId);

            coupon.UsedCount++;
            _unitOfWork.GetRepository<DiscountCoupon, Guid>().Update(coupon);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
