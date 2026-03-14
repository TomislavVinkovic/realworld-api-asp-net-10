namespace dotnet_api_tutorial.Models;

public class User : BaseEntity
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    
    public string? Bio { get; set; }
    
    public string? Image { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    // Relationships
    public ICollection<Article> FavoritedArticles { get; set; } = new List<Article>();
    public ICollection<Article> WrittenArticles { get; set; } = new List<Article>();
    public ICollection<User> Following { get; set; } = new List<User>();
    public ICollection<User> Followers { get; set; } = new List<User>();
}