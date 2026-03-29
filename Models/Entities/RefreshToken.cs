namespace RealWorld.Models.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string Token { get; set; } = string.Empty;  // SHA-256 hash
    public string Family { get; set; } = string.Empty; // groups rotation chain
    public bool IsRevoked { get; set; }
    public DateTime ExpiryTime { get; set; }
    public DateTime CreatedAt { get; set; }
}