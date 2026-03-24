using RealWorld.Data;
using RealWorld.Models;
using RealWorld.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace RealWorld.Services;

public class CommentService : ICommentService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextService _httpContextService;
    public CommentService
    (
        AppDbContext context,
        IHttpContextService httpContextService
    )
    {
        _context = context;
        _httpContextService = httpContextService;
    }

    public async Task<IEnumerable<CommentDto>?> GetCommentsForArticleAsync(string slug)
    {
        var article = await _context.Articles
            .Include(a => a.Comments)
                .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(a => a.Slug == slug);

        if(article == null)
        {
            return null;
        }

        var followedAuthorIds = new HashSet<int>();
        var commenterIds = article.Comments.Select(c => c.Author.Id).Distinct().ToList();

        var currentUserId = _httpContextService.GetCurrentUserId();

        if(currentUserId.HasValue)
        {
            var followedList = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == currentUserId)
                .SelectMany(u => u.Following)
                .Where(f => commenterIds.Contains(f.Id))
                .Select(f => f.Id)
                .ToListAsync();

            // Put them in a HashSet for lightning-fast lookups in the next step
            followedAuthorIds = new HashSet<int>(followedList);

            return article.Comments.Select(c => 
                new CommentDto(c, followedAuthorIds.Contains(c.Author.Id))
            );
        }
        
        return article.Comments.Select(c => new CommentDto(c, false));
    }

    public async Task<CommentDto?> CreateAsync(CreateCommentDto dto, string slug)
    {
        var currentUserId = _httpContextService.GetCurrentUserId()!;

        var article = await _context.Articles.FirstOrDefaultAsync(a => a.Slug == slug);
        if(article == null)
        {
            return null;
        }

        var newComment = new Comment
        {
            Body = dto.Body,
            AuthorId = (int)currentUserId,
            ArticleId = article.Id
        };
        _context.Comments.Add(newComment);

        await _context.SaveChangesAsync();
        await _context.Entry(newComment).Reference(c => c.Author).LoadAsync();

        return new CommentDto(newComment, false);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        int? currentUserId = _httpContextService.GetCurrentUserId();

        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null)
        {
            return false;
        }
        if (comment.AuthorId != currentUserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this article.");
        }

        await _context.Comments
            .Where(a => a.Id == id)
            .ExecuteDeleteAsync();

        return true;
    }
}