using RealWorld.Models;

namespace RealWorld.DTOs;

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

public class AuthorDto
{
    public AuthorDto(User user, bool following)
    {
        Username = user.Username;
        Bio = user.Bio;
        Image = user.Image;
        Following = following;
    }
    public string Username {get; set;}
    public string? Bio {get; set;} = "";
    public string? Image {get; set;} = "";
    public bool Following {get; set;}
}
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

public record ArticleResponse(ArticleDto Article);
public record ArticleListResponse(IEnumerable<ArticleDto> articles, int articlesCount);