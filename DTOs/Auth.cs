namespace dotnet_api_tutorial.DTOs;

public record RegisterDto(string Username, string Email, string Password);
public record RegisterRequest(RegisterDto user);

public record LoginDto(string Email, string Password);
public record LoginRequest(LoginDto user);

public record TokenRequest(string AccessToken, string RefreshToken);
public record UpdateUserDto(string? Email, string? Username, string? Bio, string? Image, string? Password);
public record UpdateUserRequest(UpdateUserDto user);

// What we send back to the user (notice we include the JWT token here)
public record UserResponse(string Email, string Token, string RefreshToken, string Username, string? Bio, string? Image);