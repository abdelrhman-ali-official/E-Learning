namespace Domain.Entities.CourseEntities
{
   
    public class CoursePaymentRequest : BaseEntity<int>
    {
        public Guid CourseId { get; set; }
        public string StudentId { get; set; } = string.Empty;

        public int EWalletMethodId { get; set; }

        public decimal Amount { get; set; }

        public string StudentWalletNumber { get; set; } = string.Empty;

        public string ScreenshotUrl { get; set; } = string.Empty;

        public CoursePaymentStatus Status { get; set; } = CoursePaymentStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedBy { get; set; }
        public string? RejectionReason { get; set; }

        public Course Course { get; set; } = null!;
        public CourseEWalletMethod EWalletMethod { get; set; } = null!;
    }

    public enum CoursePaymentStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }
}
