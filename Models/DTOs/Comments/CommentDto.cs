using RealWorld.Models.Entities;
using RealWorld.Models.DTOs.Articles;

namespace RealWorld.Models.DTOs.Comments;

public class CommentDto
{
    public int Id { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    public string Body { get; set; } = "";
    public AuthorDto Author { get; set; }
    public bool Following { get; set; }
}