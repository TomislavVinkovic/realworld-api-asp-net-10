using RealWorld.Models;

namespace RealWorld.DTOs.Articles;

public class ArticleDto
{
    public ArticleDto(Article article, bool isFavorited, bool isFollowing)
    {
        Title = article.Title;
        Description = article.Description;
        Body = article.Body;
        Slug = article.Slug;
        TagList = article.TagList.Select(t => t.TagText).ToArray();
        Author = new AuthorDto(article.Author, isFollowing);
        FavoritesCount = article.FavoritedBy.Count();
        Favorited = isFavorited;

        CreatedAt = article.CreatedAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        UpdatedAt = article.UpdatedAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }

    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Body { get; set; } = "";
    public string Slug { get; set; } = "";
    public AuthorDto Author { get; set; }
    public string[] TagList { get; set; } = [];
    public bool Favorited { get; set; }
    public int FavoritesCount { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
}