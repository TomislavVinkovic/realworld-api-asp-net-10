namespace dotnet_api_tutorial.DTOs;

public class ArticleQueryParameters
{
    public string? Tag { get; set; }
    public string? Author { get; set; }
    public string? Favorited { get; set; }
    public int Limit { get; set; } = 20;
    public int Offset { get; set; } = 0;
}

public class CreateArticleDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Body { get; set; }
    public string[] TagList { get; set; } = [];
}
public record CreateArticleRequest(CreateArticleDto article);

public class UpdateArticleDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Body { get; set; }
    public string[] TagList { get; set; } = [];
}
public record UpdateArticleRequest(UpdateArticleDto article);
