using RealWorld.Data;
using RealWorld.DTOs;
using RealWorld.Models;
using RealWorld.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace RealWorld.Services;


public class ArticleService : IArticleService {

    private readonly AppDbContext _context;
    private readonly IHttpContextService _httpContextService;
    private readonly IFileService _fileService;

    public ArticleService(
        AppDbContext context,
        IHttpContextService httpContextService,
        IFileService fileService
    )
    {
        _context = context;
        _httpContextService = httpContextService;
        _fileService = fileService;
    }

    public async Task<(IEnumerable<ArticleDto> articles, int Count)> GetArticlesAsync(
        ArticleQueryParameters query, 
        bool isFeed = false
    )
    {
        int? currentUserId = _httpContextService.GetCurrentUserId();

        var articlesQuery = _context.Articles
            .Include(a => a.Author)
                .ThenInclude(a => a.Followers)
            .Include(a => a.TagList)
            .Include(a => a.FavoritedBy)
            .AsQueryable();

        if(!string.IsNullOrEmpty(query.Author)) {
            articlesQuery = articlesQuery.Where(a => a.Author.Username == query.Author);
        }
        if(!string.IsNullOrEmpty(query.Tag))
        {
            articlesQuery = articlesQuery.Where(a => a.TagList.Any(t => t.TagText == query.Tag));
        }
        if(!string.IsNullOrWhiteSpace(query.Favorited))
        {
            articlesQuery = articlesQuery.Where(a => a.FavoritedBy.Any(f => f.Username == query.Favorited));
        }
        if(isFeed)
        {
            if(currentUserId != null)
            {
                articlesQuery = articlesQuery.Where(
                    a => a.Author.Followers.Any(f => f.Id == currentUserId)
                );
            }
            
        }

        var articleCount = await articlesQuery.CountAsync();
        var articles = await articlesQuery
            .OrderByDescending(a => a.CreatedAt)
            .Skip(query.Offset)
            .Take(query.Limit)
            .ToListAsync();

        var returnArticles = articles.Select(
            a =>
            {
                bool isFavorited = false;
                bool isFollowing = false;
                if(currentUserId != null)
                {
                    isFavorited = a.FavoritedBy.FirstOrDefault(u => u.Id == currentUserId) != null;
                    isFollowing = a.Author.Followers.FirstOrDefault(u => u.Id == currentUserId) != null;
                }                
                
                return ArticleDtoFactory(a, isFavorited, isFollowing);
            }
        );

        return new (returnArticles, articleCount);
    }

    public async Task<ArticleDto?> GetArticleBySlugAsync(string slug)
    {
        var currentUserId = _httpContextService.GetCurrentUserId();

        var article = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.FavoritedBy)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Slug == slug);
        
        if(article == null)
        {
            return null;
        }

        bool isFavorited = false;
        bool isFollowing = false;
        if(currentUserId != null)
        {
            isFollowing = article.Author.Followers.FirstOrDefault(u => u.Id == currentUserId) != null;
            isFavorited = article.FavoritedBy.FirstOrDefault(u => u.Id == currentUserId) != null;
        }

        return ArticleDtoFactory(article, isFavorited, isFollowing);
    }

    public async Task<ArticleDto> CreateAsync(CreateArticleDto dto)
    {
        int? currentUserId = _httpContextService.GetCurrentUserId();
        var currentUser = _context.Users.Find(currentUserId);
        
        string slug = await SlugifyAsync(dto.Title);

        Article article = new Article
        {
            Title = dto.Title,
            Description = dto.Description,
            Body = dto.Body,
            Slug = slug, 
            AuthorId = (int) currentUserId
        };
        
        if(dto.TagList.Any())
        {
            var incomingTags = dto.TagList.Select(t => t.ToLower()).Distinct().ToList();
            var existingTags = await _context.Tags
                .Where(t => incomingTags.Contains(t.TagText))
                .ToListAsync();
            
            var existingTagNames = existingTags.Select(t => t.TagText).ToList();
            var newTagNames = dto.TagList.Except(existingTagNames);

            var newTags = newTagNames.Select(name => new Tag {TagText = name}).ToList();
            article.TagList = existingTags.Concat(newTags).ToList();
        }

        // The user automatically favorites their own article
        article.FavoritedBy.Add(currentUser!);

        _context.Articles.Add(article);
        await _context.SaveChangesAsync();

        await _context.Entry(article).Reference(a => a.Author).LoadAsync();

        return ArticleDtoFactory(article, true, false);
    }

    public async Task<ArticleDto?> UpdateAsync(string slug, UpdateArticleDto dto)
    {
        int? currentUserId = _httpContextService.GetCurrentUserId();

        var article = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.FavoritedBy)
            .Where(a => a.Slug == slug)
            .FirstOrDefaultAsync();
        
        if(article == null)
        {
            return null;
        }
        if (article.AuthorId != currentUserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to edit this article.");
        }

        // Apply partial updates
        if(!string.IsNullOrWhiteSpace(dto.Title))
        {
            article.Title = dto.Title;
            article.Slug = await SlugifyAsync(dto.Title);
        }
        if(!string.IsNullOrWhiteSpace(dto.Description))
        {
            article.Description = dto.Description;
        }
        if(!string.IsNullOrWhiteSpace(dto.Body))
        {
            article.Body = dto.Body;
        }

        await _context.SaveChangesAsync();

        bool isFavorited = article.FavoritedBy.FirstOrDefault(u => u.Id == currentUserId) != null;
        return ArticleDtoFactory(article, isFavorited, false);
    }

    public async Task<bool> DeleteAsync(string slug)
    {
        int? currentUserId = _httpContextService.GetCurrentUserId();

        var authorId = await _context.Articles
            .Where(a => a.Slug == slug)
            .Select(a => (int?)a.AuthorId)
            .FirstOrDefaultAsync();
        if (authorId == null)
        {
            return false;
        }
        if (authorId != currentUserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this article.");
        }

        await _context.Articles
            .Where(a => a.Slug == slug)
            .ExecuteDeleteAsync();

        return true;
    }

    public async Task<ArticleDto?> FavoriteArticleAsync(string slug)
    {
        int? currentUserId = _httpContextService.GetCurrentUserId();
        var article = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.TagList)
            .Include(a => a.FavoritedBy)
            .FirstOrDefaultAsync(a => a.Slug == slug);

        if (article == null)
        {
            return null;
        }

        if(!article.FavoritedBy.Any(u => u.Id == currentUserId))
        {
            var user = await _context.Users.FindAsync(currentUserId);
            article.FavoritedBy.Add(user!);
            await _context.SaveChangesAsync();
        }
        bool isFollowing = article.Author.Followers.FirstOrDefault(u => u.Id == currentUserId) != null;
        return ArticleDtoFactory(article, true, isFollowing);
    }

    public async Task<ArticleDto?> UnfavoriteArticleAsync(string slug)
    {
        int? currentUserId = _httpContextService.GetCurrentUserId();
        var article = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.TagList)
            .Include(a => a.FavoritedBy)
            .FirstOrDefaultAsync(a => a.Slug == slug);

        if (article == null)
        {
            return null;
        }

        if(article.FavoritedBy.Any(u => u.Id == currentUserId))
        {
            var user = await _context.Users.FindAsync(currentUserId);
            article.FavoritedBy.Remove(user!);
            await _context.SaveChangesAsync();
        }
        bool isFollowing = article.Author.Followers.FirstOrDefault(u => u.Id == currentUserId) != null;
        return ArticleDtoFactory(article, false, isFollowing);
    }

    private ArticleDto ArticleDtoFactory(Article article, bool isFavorited, bool isFollowing)
    {
        var a = new ArticleDto(article, isFavorited, isFollowing);
        
        // Manufacture the profile image URL
        var absoluteFileUrl = _fileService.GetAbsoluteFileUrl(article.Author.Image);
        a.Author.Image = absoluteFileUrl;

        return a;
    }

    private async Task<string> SlugifyAsync(string text)
    {
        string slug = text.Replace(' ', '-').ToLower();

        bool slugExists = await _context.Articles.AnyAsync(a => a.Slug == slug);

        if(slugExists)
        {
            var randomSuffix = Guid.NewGuid().ToString().Substring(0, 6);
            slug = $"{slug}-{randomSuffix}";
        }
        return slug;
    }
}