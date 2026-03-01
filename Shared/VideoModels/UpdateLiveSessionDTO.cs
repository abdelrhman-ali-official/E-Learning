using System.ComponentModel.DataAnnotations;

namespace Shared.VideoModels;

public record UpdateLiveSessionDTO
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; init; } = string.Empty;

    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string Description { get; init; } = string.Empty;

    [MaxLength(50, ErrorMessage = "YouTube Live Video ID cannot exceed 50 characters")]
    public string? YouTubeLiveVideoId { get; init; }

    [Required(ErrorMessage = "Scheduled start time is required")]
    public DateTime ScheduledStart { get; init; }

    [Required(ErrorMessage = "Scheduled end time is required")]
    public DateTime ScheduledEnd { get; init; }
}
