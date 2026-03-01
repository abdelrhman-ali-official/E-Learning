using System.ComponentModel.DataAnnotations;

namespace Shared.VideoModels;

public record CreateLiveSessionDTO
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; init; } = string.Empty;

    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string Description { get; init; } = string.Empty;

    [Required(ErrorMessage = "Scheduled start time is required")]
    public DateTime ScheduledStart { get; init; }

    [Required(ErrorMessage = "Scheduled end time is required")]
    public DateTime ScheduledEnd { get; init; }
}

