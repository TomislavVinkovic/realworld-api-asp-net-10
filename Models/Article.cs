using dotnet_api_tutorial.Services;

namespace dotnet_api_tutorial.Models;

public class Article : BaseEntity
{
    public int Id { get; set; }
    required public string Slug { get; set; }
    required public string Title { get; set; }
    public string? Description { get; set; }
    public string? Body { get; set; }

    // Relationships
    public int AuthorId { get; set; }
    public User Author { get; set; }
    public ICollection<Tag> TagList { get; set; } = new List<Tag>();
    public ICollection<User> FavoritedBy { get; set; } = new List<User>();
    public ICollection<Comment> Comments {get; set;} = new List<Comment>();
}