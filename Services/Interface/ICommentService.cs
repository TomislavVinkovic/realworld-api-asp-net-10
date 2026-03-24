using RealWorld.DTOs.Comments;

namespace RealWorld.Services.Interface;

public interface ICommentService
{
    public Task<IEnumerable<CommentDto>?> GetCommentsForArticleAsync(string slug);
    public Task<CommentDto?> CreateAsync(CreateCommentDto dto, string slug);
    public Task<bool> DeleteAsync(int id);
}