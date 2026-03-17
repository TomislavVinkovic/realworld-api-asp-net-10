using dotnet_api_tutorial.DTOs;
using dotnet_api_tutorial.Models;

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
public record CommentResponse(CommentDto comment);
public record CommentListResponse(IEnumerable<CommentDto> comments);

public record CreateCommentDto(string Body);
public record CreateCommentRequest(CreateCommentDto comment);