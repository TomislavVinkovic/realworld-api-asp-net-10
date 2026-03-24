namespace RealWorld.DTOs.Auth;

public record UpdateUserDto
{
    public string? Email { get; init; }
    public string? Username { get; init; }
    public string? Bio { get; init; }
    public string? Image { get; init; }
    public string? Password { get; init; }
}