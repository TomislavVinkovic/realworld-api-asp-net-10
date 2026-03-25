namespace RealWorld.Models.DTOs.Articles;

public class ArticleDto
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Body { get; set; } = "";
    public string Slug { get; set; } = "";
    public AuthorDto Author { get; set; }
    public List<string> TagList { get; set; } = new List<string>();
    public bool Favorited { get; set; }
    public int FavoritesCount { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
}