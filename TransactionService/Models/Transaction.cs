using System.ComponentModel.DataAnnotations.Schema;

namespace TransactionService.Models;

public class Transaction
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string TransactionType { get; set; } = "Deposit";

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public string Currency { get; set; } = "INR";

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    public string Description { get; set; } = string.Empty;
}
