namespace RealWorld.DTOs.Articles;

public class UpdateArticleDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Body { get; set; }
    public List<string> TagList { get; set; } = new List<string>();
}