using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Shared.ContentModels
{
    public record CreateManualPaymentRequestDTO
    {
        [Required(ErrorMessage = "Content ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid content ID")]
        public int ContentId { get; init; }

        [Required(ErrorMessage = "Transfer method is required")]
        [Range(1, 2, ErrorMessage = "Transfer method must be InstaPay (1) or VodafoneCash (2)")]
        public int TransferMethod { get; init; }

        [Required(ErrorMessage = "Reference number is required")]
        [MaxLength(100)]
        public string ReferenceNumber { get; init; } = string.Empty;
    }
}
