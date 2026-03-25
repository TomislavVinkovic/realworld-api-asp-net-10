namespace RealWorld.Models.DTOs.Auth;

public record EditUserDto
{
    public string? Email { get; init; }
    public string? Username { get; init; }
    public string? Bio { get; init; }
    public string? Image { get; init; }

}