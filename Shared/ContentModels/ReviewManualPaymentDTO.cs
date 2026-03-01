using System.ComponentModel.DataAnnotations;

namespace Shared.ContentModels
{
    public record ReviewManualPaymentDTO
    {
        [Required(ErrorMessage = "Status is required (2 = Approved, 3 = Rejected)")]
        [Range(2, 3, ErrorMessage = "Status must be Approved (2) or Rejected (3)")]
        public int Status { get; init; }

        [MaxLength(500)]
        public string? RejectionReason { get; init; }
    }
}
