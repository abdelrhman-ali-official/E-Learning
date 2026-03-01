using FluentValidation;
using Shared.CourseModels;

namespace Services.Validators
{
    public class CreateCourseValidator : AbstractValidator<CreateCourseDTO>
    {
        public CreateCourseValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Course title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(5000).WithMessage("Description cannot exceed 5000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative")
                .LessThanOrEqualTo(100000).WithMessage("Price cannot exceed 100,000");

            RuleFor(x => x.EstimatedDurationMinutes)
                .GreaterThan(0).WithMessage("Duration must be greater than 0 minutes")
                .LessThanOrEqualTo(50000).WithMessage("Duration cannot exceed 50,000 minutes");

            RuleFor(x => x.Level)
                .Must(BeValidLevel).When(x => !string.IsNullOrEmpty(x.Level))
                .WithMessage("Level must be one of: Beginner, Intermediate, Advanced");

            // When IsFree is true, Price should be 0
            RuleFor(x => x.Price)
                .Equal(0).When(x => x.IsFree)
                .WithMessage("Free courses must have Price = 0");
        }

        private bool BeValidLevel(string? level)
        {
            if (string.IsNullOrEmpty(level)) return true;
            
            var validLevels = new[] { "Beginner", "Intermediate", "Advanced" };
            return validLevels.Contains(level, StringComparer.OrdinalIgnoreCase);
        }
    }
}
