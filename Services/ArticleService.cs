using System.Security.Claims;
using dotnet_api_tutorial.Data;
using dotnet_api_tutorial.DTOs;
using dotnet_api_tutorial.Models;
using dotnet_api_tutorial.Services.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Namespace;

namespace dotnet_api_tutorial.Services;


public class ArticleService : IArticleService {

    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ArticleService(
        AppDbContext context,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<(IEnumerable<Article> articles, int Count)> GetArticlesAsync(
        ArticleQueryParameters query, 
        bool isFeed = false
    )
    {
        var articlesQuery = _context.Articles
            .Include(a => a.Author)
            .Include(a => a.TagList)
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
            var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
            int? currentUserId = string.IsNullOrEmpty(userIdString) ? null : int.Parse(userIdString);
            
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

        return new (articles, articleCount);
    }

    public async Task<Article?> GetArticleBySlugAsync(string slug)
    {
        var article = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.FavoritedBy)
            .FirstOrDefaultAsync(a => a.Slug == slug);


        return article;
    }

    public async Task<Article> CreateAsync(CreateArticleDto dto)
    {
        var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
        int currentUserId = int.Parse(userIdString);

        Article article = new Article
        {
            Title = dto.Title,
            Description = dto.Description,
            Body = dto.Body,
            Slug = dto.Title.Replace(' ', '-').ToLower(),  
            AuthorId = currentUserId
        };
        
        // TODO: Extract this into a seperate tag service
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

        _context.Articles.Add(article);
        await _context.SaveChangesAsync();
        return article;
    }

    public async Task<Article?> UpdateAsync(string slug, UpdateArticleDto dto)
    {
        var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
        int currentUserId = int.Parse(userIdString);

        var article = await _context.Articles
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
            // TODO: create a private method to generate the slug in multiple places
            article.Slug = dto.Title.Replace(' ', '-').ToLower();
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

        return article;
    }

    public async Task<bool> DeleteAsync(string slug)
    {
        var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        int currentUserId = int.Parse(userIdString!);

            var article = await _context.Articles.FirstOrDefaultAsync(a => a.Slug == slug);

        if (article == null)
        {
            return false;
        }

        if (article.AuthorId != currentUserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this article.");
        }

        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();

        return true;
    }
}