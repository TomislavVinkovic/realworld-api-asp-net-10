using RealWorld.Common;
using RealWorld.Models.DTOs.Comments;

namespace RealWorld.Services.Interface;

public interface ICommentService
{
    public Task<ServiceResult<CommentListResponse?>> GetCommentsForArticleAsync(string slug);
    public Task<ServiceResult<CommentResponse?>> CreateAsync(CreateCommentDto dto, string slug);
    public Task<ServiceResult<bool>> DeleteAsync(int id);
}