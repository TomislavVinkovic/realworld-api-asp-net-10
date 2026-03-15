namespace dotnet_api_tutorial.DTOs;

public record ProfileDto(string Username, string? Bio, string? Image, bool Following);
public record ProfileResponse(ProfileDto profile);