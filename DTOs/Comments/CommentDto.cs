using RealWorld.Models;
using RealWorld.DTOs.Articles;

namespace RealWorld.DTOs.Comments;

public class CommentDto
{
    public CommentDto(Comment comment, bool isFollowingAuthor)
    {
        Id = comment.Id;
        Body = comment.Body;
        Author = new AuthorDto(comment.Author, isFollowingAuthor);

        CreatedAt = comment.CreatedAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        UpdatedAt = comment.UpdatedAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
    public int Id {get; set;}
    public string CreatedAt {get; set;}
    public string UpdatedAt {get; set;}
    public string Body {get; set;} = "";
    public AuthorDto Author {get; set;}
    public bool Following {get; set;}
}