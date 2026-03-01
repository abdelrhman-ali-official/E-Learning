namespace Shared.CourseModels
{
    
    public class InstructorAnalyticsDTO
    {
        public int TotalCourses { get; set; }
        public int PublishedCourses { get; set; }
        public int DraftCourses { get; set; }

        public int TotalEnrollments { get; set; }
        public int ActiveEnrollments { get; set; }
        public int ExpiredEnrollments { get; set; }
        public int CertificatesIssued { get; set; }

        public double AverageRating { get; set; }           // 0.0 – 5.0
        public int TotalReviews { get; set; }
        public IDictionary<int, int> RatingDistribution { get; set; } = new Dictionary<int, int>();

        public decimal TotalRevenue { get; set; }         
        public decimal PendingRevenue { get; set; }         
        public int PendingPaymentRequests { get; set; }

        public int TotalVideos { get; set; }
        public int TotalEstimatedMinutes { get; set; }

        public CourseAnalyticsSummaryDTO? MostEnrolledCourse { get; set; }
        public CourseAnalyticsSummaryDTO? TopRatedCourse { get; set; }
        public CourseAnalyticsSummaryDTO? HighestRevenueCourse { get; set; }

        public List<CourseAnalyticsDetailDTO> CourseBreakdown { get; set; } = new();

        public List<MonthlyEnrollmentDTO> MonthlyEnrollmentTrend { get; set; } = new();
    }

    public class CourseAnalyticsSummaryDTO
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Enrollments { get; set; }
        public double AverageRating { get; set; }
        public decimal Revenue { get; set; }
    }

    public class CourseAnalyticsDetailDTO
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Level { get; set; }
        public bool IsPublished { get; set; }
        public decimal Price { get; set; }
        public bool IsFree { get; set; }

        public int TotalEnrollments { get; set; }
        public int ActiveEnrollments { get; set; }
        public int CertificatesIssued { get; set; }

        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }

        public decimal TotalRevenue { get; set; }
        public int PendingPaymentRequests { get; set; }

        public int VideoCount { get; set; }
        public int EstimatedDurationMinutes { get; set; }

        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class MonthlyEnrollmentDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int Enrollments { get; set; }
    }
}
