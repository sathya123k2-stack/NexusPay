using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AccountService.Models;

public class Account
{
    [JsonRequired]
    public int Id { get; set; }

    [JsonRequired]
    public int CustomerId { get; set; }

    [Required]
    public string AccountNumber { get; set; } = string.Empty;

    [Required]
    public string AccountType { get; set; } = "Savings";

    [JsonRequired]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; }

    [Required]
    public string Currency { get; set; } = "INR";
}