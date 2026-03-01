using AutoMapper;
using Domain.Contracts;
using Domain.Entities.SubscriptionEntities;
using Services.Abstractions;
using Shared.SubscriptionModels;

namespace Services
{
    public class SubscriptionAnalyticsService : ISubscriptionAnalyticsService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubscriptionAnalyticsService(IUnitOFWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SubscriptionAnalyticsDTO> GetAnalyticsAsync(CancellationToken cancellationToken = default)
        {
            var subscriptions = await _unitOfWork.GetRepository<StudentSubscription, Guid>().GetAllAsync();
            var paymentRequests = await _unitOfWork.GetRepository<PaymentRequest, Guid>().GetAllAsync();
            var packages = await _unitOfWork.GetRepository<Package, Guid>().GetAllAsync();

            var analytics = new SubscriptionAnalyticsDTO
            {
                TotalActiveSubscriptions = subscriptions.Count(s => s.Status == SubscriptionStatus.Active),
                TotalPendingPayments = paymentRequests.Count(pr => pr.Status == PaymentStatus.Pending),
                TotalExpiredSubscriptions = subscriptions.Count(s => s.Status == SubscriptionStatus.Expired),
                TotalRevenue = paymentRequests.Where(pr => pr.Status == PaymentStatus.Approved).Sum(pr => pr.Amount),
                MonthlyRevenue = subscriptions
                    .Where(s => s.Status == SubscriptionStatus.Active && s.BillingCycle == BillingCycle.Monthly)
                    .Sum(s => s.FinalPrice),
                YearlyRevenue = subscriptions
                    .Where(s => s.Status == SubscriptionStatus.Active && s.BillingCycle == BillingCycle.Yearly)
                    .Sum(s => s.FinalPrice)
            };

            // Calculate package stats
            analytics.PackageStats = packages.Select(p => new PackageStatsDTO
            {
                PackageId = p.Id,
                PackageName = p.Name,
                SubscriptionCount = subscriptions.Count(s => s.PackageId == p.Id),
                Revenue = subscriptions
                    .Where(s => s.PackageId == p.Id && s.Status == SubscriptionStatus.Active)
                    .Sum(s => s.FinalPrice)
            }).ToList();

            return analytics;
        }
    }
}
