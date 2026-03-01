namespace Services.Abstractions;

/// <summary>
/// Service for validating course access permissions
/// Centralized guard for purchase and subscription validation
/// </summary>
public interface ICourseAccessService
{
    /// <summary>
    /// Validates if user has access to a course
    /// Checks:
    /// - Course purchase exists
    /// - Subscription not expired (if applicable)
    /// Throws CourseAccessDeniedException if access denied
    /// </summary>
    Task ValidateCourseAccessAsync(Guid courseId, string userId);

    /// <summary>
    /// Validates if user has access to content by content ID
    /// </summary>
    Task ValidateContentAccessAsync(int contentId, string userId);

    /// <summary>
    /// Checks if user has purchased a course
    /// </summary>
    Task<bool> HasCoursePurchaseAsync(Guid courseId, string userId);

    /// <summary>
    /// Checks if user has purchased content by content ID
    /// </summary>
    Task<bool> HasContentPurchaseAsync(int contentId, string userId);

    /// <summary>
    /// Checks if user has active subscription (if subscription system exists)
    /// </summary>
    Task<bool> HasActiveSubscriptionAsync(string userId);
}
