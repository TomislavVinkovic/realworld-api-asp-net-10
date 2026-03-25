namespace RealWorld.DTOs.Auth;

public record UpdateUserDto
{
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? Bio { get; set; }
    public string? Image { get; set; }
    public string? Password { get; set; }
}