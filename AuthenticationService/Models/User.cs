using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Models;

public class User
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [System.Text.Json.Serialization.JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "Customer";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
