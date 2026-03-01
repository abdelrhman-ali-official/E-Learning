namespace Domain.Entities.CourseEntities
{
    public class Enrollment : BaseEntity<Guid>
    {
        public Guid CourseId { get; set; }
        public string StudentId { get; set; } = string.Empty; 
        
        public EnrollmentSource Source { get; set; }
        
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; } 
        public bool IsActive { get; set; } = true;
        
        public int ProgressPercentage { get; set; } = 0;
        public int CompletedVideos { get; set; } = 0;
        public int TotalVideos { get; set; } = 0;
        public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsCertificateIssued { get; set; } = false;
        public DateTime? CertificateIssuedAt { get; set; }
        
        
    }
    
    public enum EnrollmentSource
    {
        Purchase = 1,     
        Subscription = 2, 
        Free = 3,          
        AdminGrant = 4, 
        Coupon = 5         
    }
}
