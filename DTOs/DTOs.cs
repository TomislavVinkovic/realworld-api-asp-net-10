using dotnet_api_tutorial.Models;

namespace dotnet_api_tutorial.DTOs;

public record RegisterRequest(string Username, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record TokenRequest(string AccessToken, string RefreshToken);

// What we send back to the user (notice we include the JWT token here)
public record UserResponse(string Email, string Token, string RefreshToken, string Username, string? Bio, string? Image);