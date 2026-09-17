using System.ComponentModel.DataAnnotations.Schema;
namespace AccountService.Models;

public class Account
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountType { get; set; } = "Savings";
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; }

    public string Currency { get; set; } = "INR";
}
