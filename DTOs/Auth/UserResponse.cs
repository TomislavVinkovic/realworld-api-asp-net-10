namespace RealWorld.DTOs.Auth;


public record UserResponse (
    string Email, 
    string Token, 
    string RefreshToken, 
    string Username, 
    string? Bio, 
    string? Image
);