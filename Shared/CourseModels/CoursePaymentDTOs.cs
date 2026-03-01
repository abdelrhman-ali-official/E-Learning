using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Shared.CourseModels
{
    public record AddEWalletMethodDTO
    {
        [Required, MaxLength(100)]
        public string MethodName { get; init; } = string.Empty; // e.g. "Vodafone Cash"

        [Required, MaxLength(50)]
        public string WalletNumber { get; init; } = string.Empty;
    }

    public record EWalletMethodDTO
    {
        public int Id { get; init; }
        public Guid CourseId { get; init; }
        public string MethodName { get; init; } = string.Empty;
        public string WalletNumber { get; init; } = string.Empty;
        public bool IsActive { get; init; }
    }

    public record SubmitCoursePaymentDTO
    {
        [Required]
        public int PaymentMethod { get; init; }

        [Required, MaxLength(50)]
        public string StudentWalletNumber { get; init; } = string.Empty;

        public IFormFile? PaymentProof { get; init; }
    }

    public record CoursePaymentRequestDTO
    {
        public int Id { get; init; }
        public Guid CourseId { get; init; }
        public string CourseTitle { get; init; } = string.Empty;
        public string StudentId { get; init; } = string.Empty;
        public string StudentName { get; init; } = string.Empty;
        public string StudentEmail { get; init; } = string.Empty;
        public string MethodName { get; init; } = string.Empty;
        public string InstructorWalletNumber { get; init; } = string.Empty;
        public string StudentWalletNumber { get; init; } = string.Empty;
        public string ScreenshotUrl { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public string Status { get; init; } = string.Empty; // Pending / Approved / Rejected
        public DateTime CreatedAt { get; init; }
        public DateTime? ReviewedAt { get; init; }
        public string? ReviewedBy { get; init; }
        public string? RejectionReason { get; init; }
    }

    public record ReviewCoursePaymentDTO
    {
        [Required]
        public bool Approve { get; init; }

        [MaxLength(500)]
        public string? RejectionReason { get; init; }
    }
}
