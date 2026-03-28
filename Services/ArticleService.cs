using RealWorld.Data;
using RealWorld.Models.Entities;
using RealWorld.Services.Interface;
using Microsoft.EntityFrameworkCore;
using RealWorld.Models.DTOs.Articles;
using Mapster;
using RealWorld.Common;

namespace RealWorld.Services;


public class ArticleService : IArticleService {

    private readonly AppDbContext _context;
    private readonly IFileService _fileService;

    public ArticleService(
        AppDbContext context,
        IFileService fileService
    )
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<ServiceResult<ArticleListResponse>> GetArticlesAsync(
        ArticleQueryParameters query, 
        bool isFeed = false,
        int? userId = null
    )
    {
        var articlesQuery = _context.Articles
            .Include(a => a.Author)
                .ThenInclude(a => a.Followers)
            .Include(a => a.TagList)
            .Include(a => a.FavoritedBy)
            .AsQueryable();

        if(!string.IsNullOrEmpty(query.Author)) 
        {
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
            if(userId != null)
            {
                articlesQuery = articlesQuery.Where(
                    a => a.Author.Followers.Any(f => f.Id == userId)
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
                if(userId != null)
                {
                    isFavorited = a.FavoritedBy.FirstOrDefault(u => u.Id == userId) != null;
                    isFollowing = a.Author.Followers.FirstOrDefault(u => u.Id == userId) != null;
                }                
                
                return ArticleDtoFactory(a, isFavorited, isFollowing);
            }
        );

        var response = new ArticleListResponse(
            returnArticles, 
            articleCount
        );
        return ServiceResult<ArticleListResponse>.Ok(response);
    }

    public async Task<ServiceResult<ArticleResponse?>> GetArticleBySlugAsync(string slug, int? userId)
    {
        var article = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.FavoritedBy)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Slug == slug);
        
        if(article == null)
        {
            return ServiceResult<ArticleResponse?>.NotFound($"Article with slug '{slug}' was not found.");
        }

        bool isFavorited = false;
        bool isFollowing = false;
        if(userId != null)
        {
            isFollowing = article.Author.Followers.FirstOrDefault(u => u.Id == userId) != null;
            isFavorited = article.FavoritedBy.FirstOrDefault(u => u.Id == userId) != null;
        }
        
        var dto = ArticleDtoFactory(article, isFavorited, isFollowing);
        var response = new ArticleResponse(dto);

        return ServiceResult<ArticleResponse?>.Ok(response);
    }

    public async Task<ServiceResult<ArticleResponse>> CreateAsync(CreateArticleDto dto, int userId)
    {
        var currentUser = _context.Users.Find(userId);
        
        string slug = await SlugifyAsync(dto.Title);

        Article article = new Article
        {
            Title = dto.Title,
            Description = dto.Description,
            Body = dto.Body,
            Slug = slug, 
            AuthorId = userId
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

        var data = ArticleDtoFactory(article, true, false);
        var response = new ArticleResponse(data);

        return ServiceResult<ArticleResponse>.Ok(response);
    }

    public async Task<ServiceResult<ArticleResponse?>> UpdateAsync(
        string slug, 
        UpdateArticleDto dto,
        int userId
    )
    {
        var article = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.FavoritedBy)
            .Where(a => a.Slug == slug)
            .FirstOrDefaultAsync();
        
        if(article == null)
        {
            return ServiceResult<ArticleResponse?>.NotFound($"Article with slug '{slug}' was not found.");
        }
        if (article.AuthorId != userId)
        {
            return ServiceResult<ArticleResponse?>.Unauthorized("You do not have permission to edit this article.");
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

        bool isFavorited = article.FavoritedBy.FirstOrDefault(u => u.Id == userId) != null;

        var articleDto = ArticleDtoFactory(article, isFavorited, false);
        var response = new ArticleResponse(articleDto);

        return ServiceResult<ArticleResponse?>.Ok(response);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string slug, int userId)
    {
        var authorId = await _context.Articles
            .Where(a => a.Slug == slug)
            .Select(a => (int?)a.AuthorId)
            .FirstOrDefaultAsync();
        if (authorId == null)
        {
            return ServiceResult<bool>.NotFound($"Article with slug '{slug}' was not found.");
        }
        if (authorId != userId)
        {
            return ServiceResult<bool>.Unauthorized("You do not have permission to delete this article");
        }

        await _context.Articles
            .Where(a => a.Slug == slug)
            .ExecuteDeleteAsync();

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<ArticleResponse?>> FavoriteArticleAsync(string slug, int userId)
    {
        var article = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.TagList)
            .Include(a => a.FavoritedBy)
            .FirstOrDefaultAsync(a => a.Slug == slug);

        if (article == null)
        {
            return ServiceResult<ArticleResponse?>.NotFound($"Article with slug '{slug}' was not found.");
        }

        if(!article.FavoritedBy.Any(u => u.Id == userId))
        {
            var user = await _context.Users.FindAsync(userId);
            article.FavoritedBy.Add(user!);
            await _context.SaveChangesAsync();
        }
        bool isFollowing = article.Author.Followers.FirstOrDefault(u => u.Id == userId) != null;
        
        var dto = ArticleDtoFactory(article, true, isFollowing);
        var response = new ArticleResponse(dto);

        return ServiceResult<ArticleResponse?>.Ok(response);
    }

    public async Task<ServiceResult<ArticleResponse?>> UnfavoriteArticleAsync(string slug, int userId)
    {
        var article = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.TagList)
            .Include(a => a.FavoritedBy)
            .FirstOrDefaultAsync(a => a.Slug == slug);

        if (article == null)
        {
            ServiceResult<ArticleDto?>.NotFound($"Article with slug '{slug}' was not found.");
        }

        if(article!.FavoritedBy.Any(u => u.Id == userId))
        {
            var user = await _context.Users.FindAsync(userId);
            article.FavoritedBy.Remove(user!);
            await _context.SaveChangesAsync();
        }
        bool isFollowing = article.Author.Followers.FirstOrDefault(u => u.Id == userId) != null;

        var dto = ArticleDtoFactory(article, false, isFollowing);
        var response = new ArticleResponse(dto);

        return ServiceResult<ArticleResponse?>.Ok(response);
    }

    private ArticleDto ArticleDtoFactory(Article article, bool isFavorited, bool isFollowing)
    {
        var a = article.Adapt<ArticleDto>();
        a.Favorited = isFavorited;
        a.Author.Following = isFollowing;
        
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