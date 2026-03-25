namespace RealWorld.Models.DTOs.Auth;

public record TokenRequest(string AccessToken, string RefreshToken);