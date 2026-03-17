namespace dotnet_api_tutorial.Models;

public class Comment : BaseEntity
{
    public int Id {get; set;}
    public string Body {get; set;}

    // Relationships
    public int AuthorId { get; set; }
    public User Author { get; set; }
    
    public int ArticleId { get; set; }
    public Article Article {get; set;}
}