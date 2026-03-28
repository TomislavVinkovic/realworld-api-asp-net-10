using RealWorld.Common;
using RealWorld.Models.DTOs.Comments;

namespace RealWorld.Services.Interface;

public interface ICommentService
{
    public Task<ServiceResult<CommentListResponse?>> GetCommentsForArticleAsync(string slug, int? userId);
    public Task<ServiceResult<CommentResponse?>> CreateAsync(CreateCommentDto dto, string slug, int userId);
    public Task<ServiceResult<bool>> DeleteAsync(int id, int userId);
}