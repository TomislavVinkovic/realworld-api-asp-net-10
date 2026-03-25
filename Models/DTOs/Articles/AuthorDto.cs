using RealWorld.Models.Entities;

namespace RealWorld.Models.DTOs.Articles;

public class AuthorDto
{
    public string Username { get; set; }
    public string? Bio { get; set; } = "";
    public string? Image { get; set; } = "";
    public bool Following { get; set; }
}