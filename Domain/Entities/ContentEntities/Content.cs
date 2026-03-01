namespace Domain.Entities.ContentEntities
{
    public class Content : BaseEntity<int>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public ContentType Type { get; set; }
        public decimal Price { get; set; }
        public int AccessDurationWeeks { get; set; }
        public string? YoutubeVideoId { get; set; }
        public string? VideoObjectKey { get; set; }
        public string? LiveStreamUrl { get; set; }
        public string? ExternalVideoUrl { get; set; }
        public bool IsLiveActive { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool IsDownloadable { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid CourseId { get; set; }
        public string InstructorId { get; set; } = string.Empty;

        public Domain.Entities.CourseEntities.Course Course { get; set; } = null!;
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public ICollection<WatchProgress> WatchProgress { get; set; } = new List<WatchProgress>();
    }
}
