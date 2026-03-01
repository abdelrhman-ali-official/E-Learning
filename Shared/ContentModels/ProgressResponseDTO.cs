namespace Shared.ContentModels
{
    public class ProgressResponseDTO
    {
        public int LastPositionSeconds { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
