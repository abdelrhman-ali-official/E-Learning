using Domain.Entities.SecurityEntities;

namespace Domain.Entities.ContentEntities
{
    public class WatchProgress : BaseEntity<int>
    {
        public string UserId { get; set; } = string.Empty;
        public int ContentId { get; set; }
        public int LastPositionSeconds { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public Content Content { get; set; } = null!;
    }
}
