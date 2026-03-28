using RealWorld.Data;
using RealWorld.Models.Entities;
using RealWorld.Services.Interface;
using Microsoft.EntityFrameworkCore;
using RealWorld.Models.DTOs.Comments;
using Mapster;
using RealWorld.Common;

namespace RealWorld.Services;

public class CommentService : ICommentService
{
    private readonly AppDbContext _context;
    public CommentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<CommentListResponse?>> GetCommentsForArticleAsync(string slug, int? userId)
    {
        var article = await _context.Articles
            .Include(a => a.Comments)
                .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(a => a.Slug == slug);

        if(article == null)
        {
            return ServiceResult<CommentListResponse?>.NotFound($"Article with slug '{slug}' was not found.");
        }

        var followedAuthorIds = new HashSet<int>();
        var commenterIds = article.Comments.Select(c => c.Author.Id).Distinct().ToList();

        if(userId.HasValue)
        {
            var followedList = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Following)
                .Where(f => commenterIds.Contains(f.Id))
                .Select(f => f.Id)
                .ToListAsync();
            followedAuthorIds = [.. followedList];

            var commentListWithFollowing = article.Comments.Select(c =>
                CommentDtoFactory(c, followedAuthorIds.Contains(c.Author.Id))
            );
            var responseFollowing = new CommentListResponse(commentListWithFollowing);

            return ServiceResult<CommentListResponse?>.Ok(responseFollowing);
        }
        
        var commentList = article.Comments.Select(c => CommentDtoFactory(c, false));
        var response = new CommentListResponse(commentList);

        return ServiceResult<CommentListResponse?>.Ok(response);
    }

    public async Task<ServiceResult<CommentResponse?>> CreateAsync(CreateCommentDto dto, string slug, int userId)
    {
        var article = await _context.Articles.FirstOrDefaultAsync(a => a.Slug == slug);
        if(article == null)
        {
            return ServiceResult<CommentResponse?>.NotFound("Comment not found.");
        }

        var newComment = new Comment
        {
            Body = dto.Body,
            AuthorId = userId,
            ArticleId = article.Id
        };
        _context.Comments.Add(newComment);

        await _context.SaveChangesAsync();
        await _context.Entry(newComment).Reference(c => c.Author).LoadAsync();

        var commentDto = CommentDtoFactory(newComment, false);
        var response = new CommentResponse(commentDto);

        return ServiceResult<CommentResponse?>.Ok(response);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id, int userId)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null)
        {
            return ServiceResult<bool>.NotFound("Comment not found.");
        }
        if (comment.AuthorId != userId)
        {
            return ServiceResult<bool>.Unauthorized("You do not have permission to delete this article.");
        }

        await _context.Comments
            .Where(a => a.Id == id)
            .ExecuteDeleteAsync();

        return ServiceResult<bool>.Ok(true);
    }

    private CommentDto CommentDtoFactory(Comment comment, bool isFollowing)
    {
        var c = comment.Adapt<CommentDto>();
        c.Author.Following = isFollowing;

        return c;
    }
}