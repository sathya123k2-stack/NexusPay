using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentService.Models;

public class Payment
{
    public int Id { get; set; }

    [Required]
    public int AccountId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public string Currency { get; set; } = "INR";

    public string PaymentMethod { get; set; } = "UPI";

    public string PaymentStatus { get; set; } = "Pending";

    [Required]
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    public string Description { get; set; } = string.Empty;
}