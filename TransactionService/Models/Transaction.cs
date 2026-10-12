using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransactionService.Models;

public class Transaction
{
    public int Id { get; set; }

    [Required]
    public int AccountId { get; set; }

    public string TransactionType { get; set; } = "Deposit";

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public string Currency { get; set; } = "INR";

    [Required]
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    public string Description { get; set; } = string.Empty;
}