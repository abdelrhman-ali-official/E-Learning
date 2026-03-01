namespace Domain.Entities.CourseEntities
{
   
    public class CourseEWalletMethod : BaseEntity<int>
    {
        public Guid CourseId { get; set; }
        public string InstructorId { get; set; } = string.Empty;

        public string MethodName { get; set; } = string.Empty;

        public string WalletNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Course Course { get; set; } = null!;
    }
}
