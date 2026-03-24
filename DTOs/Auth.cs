using RealWorld.Models;

namespace RealWorld.DTOs;

public record RegisterDto(string Username, string Email, string Password);
public record RegisterRequest(RegisterDto user);

public record LoginDto(string Email, string Password);
public record LoginRequest(LoginDto user);

public record TokenRequest(string AccessToken, string RefreshToken);
public record UpdateUserDto
{
    public string? Email { get; init; }
    public string? Username { get; init; }
    public string? Bio { get; init; }
    public string? Image { get; init; }
    public string? Password { get; init; }
}
public class UpdateUserFormDto
{
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Bio { get; set; }
    
    // IFormFile catches the physical file from the multipart/form-data request
    public IFormFile? Image { get; set; } 
}
public class UpdateUserRequest
{
    public UpdateUserFormDto user { get; set; } = new UpdateUserFormDto(); 
}
// What we send back to the user (notice we include the JWT token here)
public record UserResponse(string Email, string Token, string RefreshToken, string Username, string? Bio, string? Image);
public record EditUserDto
{
    public string? Email { get; init; }
    public string? Username { get; init; }
    public string? Bio { get; init; }
    public string? Image { get; init; }

}
public record EditUserResponse(EditUserDto user);