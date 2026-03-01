namespace Shared.ContentModels
{
    public class ContentProgressDTO
    {
        public Guid CourseId { get; set; }
        public int TotalContents { get; set; }
        public int CompletedContents { get; set; }
        public double ProgressPercentage { get; set; }
        public List<ContentCompletionStatusDTO> Items { get; set; } = new();
    }

    public class ContentCompletionStatusDTO
    {
        public int ContentId { get; set; }
        public string ContentTitle { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }

    public class MarkContentCompleteDTO
    {
        public bool IsComplete { get; set; }
    }
}
