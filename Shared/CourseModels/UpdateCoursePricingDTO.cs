using System.ComponentModel.DataAnnotations;

namespace Shared.CourseModels
{
    public record UpdateCoursePricingDTO
    {
        [Range(0, 100000, ErrorMessage = "Price must be between 0 and 100,000")]
        public decimal? Price { get; init; }

        public bool? IsFree { get; init; }

        [Range(0, 3650, ErrorMessage = "AccessDurationDays must be between 0 (unlimited) and 3650 (10 years)")]
        public int? AccessDurationDays { get; init; }
    }
}
