using AutoMapper;
using Domain.Contracts;
using Domain.Entities.CourseEntities;
using Domain.Entities.VideoEntities;
using Services.Abstractions;
using Shared.CourseModels;

namespace Services
{
    public class InstructorAnalyticsService : IInstructorAnalyticsService
    {
        private readonly IUnitOFWork _unitOfWork;

        public InstructorAnalyticsService(IUnitOFWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<InstructorAnalyticsDTO> GetInstructorAnalyticsAsync(
            string instructorId,
            CancellationToken cancellationToken = default)
        {
            var allCourses      = (await _unitOfWork.GetRepository<Course, Guid>().GetAllAsync())
                                  .Where(c => c.InstructorId == instructorId && !c.IsDeleted)
                                  .ToList();

            var courseIds       = allCourses.Select(c => c.Id).ToHashSet();

            var allEnrollments  = (await _unitOfWork.GetRepository<Enrollment, Guid>().GetAllAsync())
                                  .Where(e => courseIds.Contains(e.CourseId))
                                  .ToList();

            var allReviews      = (await _unitOfWork.GetRepository<CourseReview, Guid>().GetAllAsync())
                                  .Where(r => courseIds.Contains(r.CourseId) && r.IsApproved && !r.IsHidden)
                                  .ToList();

            var allPayments     = (await _unitOfWork.GetRepository<CoursePaymentRequest, int>().GetAllAsync())
                                  .Where(p => courseIds.Contains(p.CourseId))
                                  .ToList();

            var allVideos       = (await _unitOfWork.GetRepository<CourseVideo, Guid>().GetAllAsync())
                                  .Where(v => courseIds.Contains(v.CourseId))
                                  .ToList();

            int totalCourses     = allCourses.Count;
            int publishedCourses = allCourses.Count(c => c.IsPublished);
            int draftCourses     = totalCourses - publishedCourses;

            int totalEnrollments  = allEnrollments.Count;
            int activeEnrollments = allEnrollments.Count(e => e.IsActive
                && (e.ExpiresAt == null || e.ExpiresAt > DateTime.UtcNow));
            int expiredEnrollments = allEnrollments.Count(e =>
                e.ExpiresAt.HasValue && e.ExpiresAt <= DateTime.UtcNow);
            int certificatesIssued = allEnrollments.Count(e => e.IsCertificateIssued);

            int totalReviews   = allReviews.Count;
            double avgRating   = totalReviews > 0
                ? Math.Round(allReviews.Average(r => r.Rating), 2)
                : 0.0;

            var ratingDistribution = Enumerable.Range(1, 5)
                .ToDictionary(star => star, star => allReviews.Count(r => r.Rating == star));

            decimal totalRevenue   = allPayments
                .Where(p => p.Status == CoursePaymentStatus.Approved)
                .Sum(p => p.Amount);
            decimal pendingRevenue = allPayments
                .Where(p => p.Status == CoursePaymentStatus.Pending)
                .Sum(p => p.Amount);
            int pendingPaymentRequests = allPayments
                .Count(p => p.Status == CoursePaymentStatus.Pending);

            int totalVideos   = allVideos.Count;
            int totalMinutes  = allCourses.Sum(c => c.EstimatedDurationMinutes);

            var courseBreakdown = allCourses.Select(course =>
            {
                var courseEnrollments = allEnrollments.Where(e => e.CourseId == course.Id).ToList();
                var courseReviews     = allReviews.Where(r => r.CourseId == course.Id).ToList();
                var coursePayments    = allPayments.Where(p => p.CourseId == course.Id).ToList();
                var courseVideos      = allVideos.Where(v => v.CourseId == course.Id).ToList();

                return new CourseAnalyticsDetailDTO
                {
                    CourseId               = course.Id,
                    Title                  = course.Title,
                    Category               = course.Category,
                    Level                  = course.Level,
                    IsPublished            = course.IsPublished,
                    Price                  = course.Price,
                    IsFree                 = course.IsFree,
                    TotalEnrollments       = courseEnrollments.Count,
                    ActiveEnrollments      = courseEnrollments.Count(e => e.IsActive
                        && (e.ExpiresAt == null || e.ExpiresAt > DateTime.UtcNow)),
                    CertificatesIssued     = courseEnrollments.Count(e => e.IsCertificateIssued),
                    TotalReviews           = courseReviews.Count,
                    AverageRating          = courseReviews.Count > 0
                        ? Math.Round(courseReviews.Average(r => r.Rating), 2) : 0.0,
                    TotalRevenue           = coursePayments
                        .Where(p => p.Status == CoursePaymentStatus.Approved).Sum(p => p.Amount),
                    PendingPaymentRequests = coursePayments.Count(p => p.Status == CoursePaymentStatus.Pending),
                    VideoCount             = courseVideos.Count,
                    EstimatedDurationMinutes = course.EstimatedDurationMinutes,
                    PublishedAt            = course.PublishedAt,
                    CreatedAt              = course.CreatedAt
                };
            }).OrderByDescending(c => c.TotalEnrollments).ToList();

            var mostEnrolled      = courseBreakdown.MaxBy(c => c.TotalEnrollments);
            var topRated          = courseBreakdown.Where(c => c.TotalReviews > 0).MaxBy(c => c.AverageRating);
            var highestRevenue    = courseBreakdown.MaxBy(c => c.TotalRevenue);

            var now          = DateTime.UtcNow;
            var twelveMonths = Enumerable.Range(0, 12)
                .Select(i => now.AddMonths(-i))
                .OrderBy(d => d)
                .ToList();

            var monthlyTrend = twelveMonths.Select(month => new MonthlyEnrollmentDTO
            {
                Year       = month.Year,
                Month      = month.Month,
                MonthName  = month.ToString("MMM yyyy"),
                Enrollments = allEnrollments.Count(e =>
                    e.EnrolledAt.Year == month.Year &&
                    e.EnrolledAt.Month == month.Month)
            }).ToList();

            return new InstructorAnalyticsDTO
            {
                TotalCourses            = totalCourses,
                PublishedCourses        = publishedCourses,
                DraftCourses            = draftCourses,

                TotalEnrollments        = totalEnrollments,
                ActiveEnrollments       = activeEnrollments,
                ExpiredEnrollments      = expiredEnrollments,
                CertificatesIssued      = certificatesIssued,

                AverageRating           = avgRating,
                TotalReviews            = totalReviews,
                RatingDistribution      = ratingDistribution,

                TotalRevenue            = totalRevenue,
                PendingRevenue          = pendingRevenue,
                PendingPaymentRequests  = pendingPaymentRequests,

                TotalVideos             = totalVideos,
                TotalEstimatedMinutes   = totalMinutes,

                MostEnrolledCourse      = ToSummary(mostEnrolled),
                TopRatedCourse          = ToSummary(topRated),
                HighestRevenueCourse    = ToSummary(highestRevenue),

                CourseBreakdown         = courseBreakdown,
                MonthlyEnrollmentTrend  = monthlyTrend
            };
        }


        private static CourseAnalyticsSummaryDTO? ToSummary(CourseAnalyticsDetailDTO? detail)
        {
            if (detail is null) return null;

            return new CourseAnalyticsSummaryDTO
            {
                CourseId      = detail.CourseId,
                Title         = detail.Title,
                Enrollments   = detail.TotalEnrollments,
                AverageRating = detail.AverageRating,
                Revenue       = detail.TotalRevenue
            };
        }
    }
}
