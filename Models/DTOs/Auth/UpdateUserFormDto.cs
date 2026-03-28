using RealWorld.Extensions.Validation;

namespace RealWorld.Models.DTOs.Auth;

public class UpdateUserFormDto
{
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Bio { get; set; }
    
    // IFormFile catches the physical file from the multipart/form-data request
    [ValidImage]
    public IFormFile? Image { get; set; } 
}