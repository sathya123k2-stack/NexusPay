using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountService.Models;

public class Account
{
    public int Id { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public string AccountNumber { get; set; } = string.Empty;

    [Required]
    public string AccountType { get; set; } = "Savings";

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; }

    [Required]
    public string Currency { get; set; } = "INR";
}