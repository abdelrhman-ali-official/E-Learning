using Domain.Contracts;
using Domain.Entities.CourseEntities;
using Domain.Entities.ProductEntities;
using Domain.Entities.ContentEntities;
using Domain.Entities.SubscriptionEntities;
using Domain.Exceptions;
using Services.Abstractions;
using Shared.Helpers;

namespace Services;

public class CourseAccessService : ICourseAccessService
{
    private readonly IUnitOFWork _unitOfWork;

    public CourseAccessService(IUnitOFWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task ValidateCourseAccessAsync(Guid courseId, string userId)
    {
        var hasPurchase = await HasCoursePurchaseAsync(courseId, userId);

        if (!hasPurchase)
        {
            var hasSubscription = await HasActiveSubscriptionAsync(userId);

            if (!hasSubscription)
            {
                throw new CourseAccessDeniedException(courseId);
            }

            var subscriptionRepo = _unitOfWork.GetRepository<StudentSubscription, Guid>();
            var allSubscriptions = await subscriptionRepo.GetAllAsync(trackChanges: false);
            
            var activeSubscription = allSubscriptions.FirstOrDefault(s => 
                s.StudentId == userId &&
                s.Status == SubscriptionStatus.Active &&
                s.EndDate > EgyptDateTime.Now);

            if (activeSubscription == null)
            {
                throw new CourseAccessDeniedException("No active subscription found");
            }

        }

    }

    public async Task ValidateContentAccessAsync(int contentId, string userId)
    {
        var hasPurchase = await HasContentPurchaseAsync(contentId, userId);

        if (!hasPurchase)
        {
            var hasSubscription = await HasActiveSubscriptionAsync(userId);

            if (!hasSubscription)
            {
                throw new CourseAccessDeniedException($"Access denied to content {contentId}. Purchase required or subscription expired.");
            }
        }
    }

    public async Task<bool> HasCoursePurchaseAsync(Guid courseId, string userId)
    {
        var enrollmentRepo = _unitOfWork.GetRepository<Enrollment, Guid>();
        var allEnrollments = await enrollmentRepo.GetAllAsync(trackChanges: false);

        return allEnrollments.Any(e =>
            e.CourseId   == courseId &&
            e.StudentId  == userId   &&
            e.IsActive   &&
            (e.ExpiresAt == null || e.ExpiresAt > EgyptDateTime.Now));
    }

    public async Task<bool> HasContentPurchaseAsync(int contentId, string userId)
    {
        var purchaseRepo = _unitOfWork.GetRepository<Purchase, int>();
        var allPurchases = await purchaseRepo.GetAllAsync(trackChanges: false);
        
        var purchase = allPurchases.Any(p =>
            p.ContentId == contentId &&
            p.UserId == userId &&
            p.IsActive &&
            p.ExpiryDate > EgyptDateTime.Now);

        return purchase;
    }

    public async Task<bool> HasActiveSubscriptionAsync(string userId)
    {
        var subscriptionRepo = _unitOfWork.GetRepository<StudentSubscription, Guid>();
        var allSubscriptions = await subscriptionRepo.GetAllAsync(trackChanges: false);
        
        var hasActive = allSubscriptions.Any(s =>
            s.StudentId == userId &&
            s.Status == SubscriptionStatus.Active &&
            s.EndDate > EgyptDateTime.Now);

        return hasActive;
    }
}
