using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentService.Models;

public class Payment
{
    public int Id { get; set; }

    public int AccountId { get; set; }
     
    [Column(TypeName = "decimal(18,2)")] 
    public decimal Amount { get; set; }

    public string Currency { get; set; } = "INR";

    public string PaymentMethod { get; set; } = "UPI";

    public string PaymentStatus { get; set; } = "Pending";

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    public string Description { get; set; } = string.Empty;
}