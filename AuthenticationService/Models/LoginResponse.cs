namespace AuthenticationService.Models;

public class LoginResponse
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}