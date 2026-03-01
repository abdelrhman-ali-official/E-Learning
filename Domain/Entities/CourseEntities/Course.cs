namespace Domain.Entities.CourseEntities
{
   
    public class Course : BaseEntity<Guid>
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty; 
        public string Description { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        
        public string InstructorId { get; set; } = string.Empty; 
        public string InstructorName { get; set; } = string.Empty;
        
        public decimal Price { get; set; }
        public bool IsFree { get; set; } = false;
        public int AccessDurationDays { get; set; } = 0;
        
        public bool IsPublished { get; set; } = false;
        public bool IsFeatured { get; set; } = false;
        public DateTime? PublishedAt { get; set; }
        
        public int EstimatedDurationMinutes { get; set; } 
        public string? Category { get; set; }
        public string? Level { get; set; } 
        
        public string? Requirements { get; set; } 
        public string? LearningObjectives { get; set; } 
        
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
    }
}
