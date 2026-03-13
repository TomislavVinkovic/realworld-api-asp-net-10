namespace dotnet_api_tutorial.Models;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    
    public string? Bio { get; set; }
    
    public string? Image { get; set; }

    // Add refresh token functionality
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}