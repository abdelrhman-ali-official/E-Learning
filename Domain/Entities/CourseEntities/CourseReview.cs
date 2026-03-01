namespace Domain.Entities.CourseEntities
{
  
    public class CourseReview : BaseEntity<Guid>
    {
        public Guid CourseId { get; set; }
        public string StudentId { get; set; } = string.Empty; 
        public int Rating { get; set; } 
        public string? ReviewText { get; set; }
        
        public bool IsApproved { get; set; } = true; 
        public bool IsHidden { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
       
    }
}
