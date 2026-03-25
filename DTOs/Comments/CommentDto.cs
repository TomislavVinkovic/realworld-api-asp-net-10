using RealWorld.Models;
using RealWorld.DTOs.Articles;

namespace RealWorld.DTOs.Comments;

public class CommentDto
{
    public int Id { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    public string Body { get; set; } = "";
    public AuthorDto Author { get; set; }
    public bool Following { get; set; }
}