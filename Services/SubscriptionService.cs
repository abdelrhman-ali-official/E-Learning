using AutoMapper;
using Domain.Contracts;
using Domain.Entities.SubscriptionEntities;
using Domain.Exceptions;
using Services.Abstractions;
using Shared.SubscriptionModels;

namespace Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICouponService _couponService;

        public SubscriptionService(IUnitOFWork unitOfWork, IMapper mapper, ICouponService couponService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _couponService = couponService;
        }

        public async Task<SubscriptionResponseDTO> SubscribeAsync(Guid studentId, SubscribeRequestDTO dto, CancellationToken cancellationToken = default)
        {
            // Check if package exists
            var package = await _unitOfWork.GetRepository<Package, Guid>().GetAsync(dto.PackageId);
            if (package == null)
                throw new PackageNotFoundException(dto.PackageId);

            if (!package.IsActive)
                throw new ValidationException(new[] { "This package is no longer available" });

            // Check if student already has an active subscription
            var existingSubscription = await GetStudentActiveSubscriptionAsync(studentId, cancellationToken);
            if (existingSubscription != null)
                throw new ValidationException(new[] { "You already have an active subscription. Please cancel it first." });

            // Calculate final price
            var finalPrice = await CalculateFinalPriceAsync(dto.PackageId, dto.BillingCycle, dto.CouponCode, cancellationToken);

            // Create subscription
            var subscription = new StudentSubscription
            {
                StudentId = studentId.ToString(),
                PackageId = dto.PackageId,
                StartDate = DateTime.UtcNow,
                BillingCycle = dto.BillingCycle,
                FinalPrice = finalPrice,
                Status = SubscriptionStatus.PendingPayment,
                CreatedAt = DateTime.UtcNow
            };

            // Set end date based on billing cycle
            subscription.EndDate = dto.BillingCycle == BillingCycle.Monthly
                ? subscription.StartDate.AddMonths(1)
                : subscription.StartDate.AddYears(1);

            // Apply coupon if provided
            if (!string.IsNullOrEmpty(dto.CouponCode))
            {
                var coupon = await _couponService.GetCouponByCodeAsync(dto.CouponCode, cancellationToken);
                subscription.DiscountCouponId = coupon.Id;
            }

            await _unitOfWork.GetRepository<StudentSubscription, Guid>().AddAsync(subscription);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SubscriptionResponseDTO>(subscription);
        }

        public async Task<SubscriptionResponseDTO> GetSubscriptionByIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
        {
            var subscription = await _unitOfWork.GetRepository<StudentSubscription, Guid>().GetAsync(subscriptionId);
            if (subscription == null)
                throw new SubscriptionNotFoundException(subscriptionId);

            return _mapper.Map<SubscriptionResponseDTO>(subscription);
        }

        public async Task<SubscriptionResponseDTO?> GetStudentActiveSubscriptionAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            var subscriptions = await _unitOfWork.GetRepository<StudentSubscription, Guid>().GetAllAsync();
            var activeSubscription = subscriptions.FirstOrDefault(s => 
                s.StudentId == studentId.ToString() && 
                s.Status == SubscriptionStatus.Active &&
                s.EndDate > DateTime.UtcNow);

            return activeSubscription == null ? null : _mapper.Map<SubscriptionResponseDTO>(activeSubscription);
        }

        public async Task<IEnumerable<SubscriptionResponseDTO>> GetAllSubscriptionsAsync(CancellationToken cancellationToken = default)
        {
            var subscriptions = await _unitOfWork.GetRepository<StudentSubscription, Guid>().GetAllAsync();
            return _mapper.Map<IEnumerable<SubscriptionResponseDTO>>(subscriptions);
        }

        public async Task<decimal> CalculateFinalPriceAsync(Guid packageId, BillingCycle billingCycle, string? couponCode = null, CancellationToken cancellationToken = default)
        {
            var package = await _unitOfWork.GetRepository<Package, Guid>().GetAsync(packageId);
            if (package == null)
                throw new PackageNotFoundException(packageId);

            // Get base price based on billing cycle
            var basePrice = billingCycle == BillingCycle.Monthly 
                ? package.PriceMonthly 
                : package.PriceYearly;

            // Apply package discount
            var priceAfterPackageDiscount = basePrice * (1 - package.DiscountPercentage / 100);

            // Apply coupon if provided
            if (!string.IsNullOrEmpty(couponCode))
            {
                var isValid = await _couponService.ValidateCouponAsync(couponCode, cancellationToken);
                if (isValid)
                {
                    var coupon = await _couponService.GetCouponByCodeAsync(couponCode, cancellationToken);
                    priceAfterPackageDiscount = priceAfterPackageDiscount * (1 - coupon.DiscountPercentage / 100);
                }
            }

            return Math.Round(priceAfterPackageDiscount, 2);
        }

        public async Task CancelSubscriptionAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
        {
            var subscription = await _unitOfWork.GetRepository<StudentSubscription, Guid>().GetAsync(subscriptionId);
            if (subscription == null)
                throw new SubscriptionNotFoundException(subscriptionId);

            subscription.Status = SubscriptionStatus.Cancelled;
            _unitOfWork.GetRepository<StudentSubscription, Guid>().Update(subscription);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ExpireSubscriptionsAsync(CancellationToken cancellationToken = default)
        {
            var subscriptions = await _unitOfWork.GetRepository<StudentSubscription, Guid>().GetAllAsync();
            
            var expiredSubscriptions = subscriptions.Where(s => 
                s.Status == SubscriptionStatus.Active && 
                s.EndDate <= DateTime.UtcNow).ToList();

            foreach (var subscription in expiredSubscriptions)
            {
                subscription.Status = SubscriptionStatus.Expired;
                _unitOfWork.GetRepository<StudentSubscription, Guid>().Update(subscription);
            }

            if (expiredSubscriptions.Any())
            {
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
