namespace RealWorld.DTOs.Profiles;

public record ProfileDto(
    string Username, 
    string? Bio, 
    string? Image, 
    bool Following
);