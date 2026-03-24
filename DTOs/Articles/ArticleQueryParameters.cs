namespace RealWorld.DTOs.Articles;

public class ArticleQueryParameters
{
    public string? Tag { get; set; }
    public string? Author { get; set; }
    public string? Favorited { get; set; }
    public int Limit { get; set; } = 20;
    public int Offset { get; set; } = 0;
}