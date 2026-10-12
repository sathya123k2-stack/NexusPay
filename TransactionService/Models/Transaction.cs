using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TransactionService.Models;

public class Transaction
{
    [JsonRequired]
    public int Id { get; set; }

    [JsonRequired]
    public int AccountId { get; set; }

    public string TransactionType { get; set; } = "Deposit";

    [JsonRequired]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public string Currency { get; set; } = "INR";

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    public string Description { get; set; } = string.Empty;
}