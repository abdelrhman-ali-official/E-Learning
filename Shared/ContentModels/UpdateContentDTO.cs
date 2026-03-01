using System.ComponentModel.DataAnnotations;

namespace Shared.ContentModels
{
    public record UpdateContentDTO
    {
        [MaxLength(200)]
        public string? Title { get; init; }

        [MaxLength(2000)]
        public string? Description { get; init; }

        public string? ThumbnailUrl { get; init; }

      
        [MaxLength(1000)]
        public string? MediaLink { get; init; }

        public bool? IsVisible { get; init; }

        
        public bool? IsDownloadable { get; init; }
    }
}
