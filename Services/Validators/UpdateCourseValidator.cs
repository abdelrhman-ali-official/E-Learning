using FluentValidation;
using Shared.CourseModels;

namespace Services.Validators
{
    public class UpdateCourseValidator : AbstractValidator<UpdateCourseDTO>
    {
        public UpdateCourseValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Description)
                .MaximumLength(5000).WithMessage("Description cannot exceed 5000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative")
                .LessThanOrEqualTo(100000).WithMessage("Price cannot exceed 100,000")
                .When(x => x.Price.HasValue);

            RuleFor(x => x.EstimatedDurationMinutes)
                .GreaterThan(0).WithMessage("Duration must be greater than 0 minutes")
                .LessThanOrEqualTo(50000).WithMessage("Duration cannot exceed 50,000 minutes")
                .When(x => x.EstimatedDurationMinutes.HasValue);

            RuleFor(x => x.Level)
                .Must(BeValidLevel).When(x => !string.IsNullOrEmpty(x.Level))
                .WithMessage("Level must be one of: Beginner, Intermediate, Advanced");
        }

        private bool BeValidLevel(string? level)
        {
            if (string.IsNullOrEmpty(level)) return true;
            
            var validLevels = new[] { "Beginner", "Intermediate", "Advanced" };
            return validLevels.Contains(level, StringComparer.OrdinalIgnoreCase);
        }
    }
}
