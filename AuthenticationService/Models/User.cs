using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthenticationService.Models;

public class User
{
    [JsonRequired]
    public int Id { get; set; }

    [JsonRequired]
    public int CustomerId { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "Customer";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}