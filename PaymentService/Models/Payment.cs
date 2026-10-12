using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PaymentService.Models;

public class Payment
{
    [JsonRequired]
    public int Id { get; set; }

    [JsonRequired]
    public int AccountId { get; set; }

    [JsonRequired]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public string Currency { get; set; } = "INR";

    public string PaymentMethod { get; set; } = "UPI";

    public string PaymentStatus { get; set; } = "Pending";

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    public string Description { get; set; } = string.Empty;
}