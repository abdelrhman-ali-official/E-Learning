using System.ComponentModel.DataAnnotations;

namespace Shared.ContentModels
{
    public record CreateContentDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200)]
        public string Title { get; init; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; init; }

      
        public bool IsDownloadable { get; init; } = false;

        public string? ThumbnailUrl { get; init; }

        [Required(ErrorMessage = "Type is required")]
        [Range(1, 7, ErrorMessage = "Type must be Video(1), Live(2), PDF(3), Image(4), Document(5), Audio(6), Other(7)")]
        public int Type { get; init; }

       
        [MaxLength(1000)]
        public string? MediaLink { get; init; }

        public bool IsVisible { get; init; } = true;
    }
}
