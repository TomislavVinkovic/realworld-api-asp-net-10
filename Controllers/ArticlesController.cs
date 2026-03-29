using RealWorld.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.Models.DTOs.Articles;
using RealWorld.Extensions;

namespace RealWorld.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ArticlesController : ApiControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController
    (
        IArticleService articleService
    )
    {
        _articleService = articleService;
    }

    [AllowAnonymous]
    [HttpGet("")]
    /// <summary>
    /// Returns a paginated list of articles, optionally filtered by the author, tags or its favorited status for the user
    /// </summary>
    /// <param name="query">Parameters for filtering and pagination</param>
    public async Task<ActionResult> List([FromQuery] ArticleQueryParameters query)
    {
        var result = await _articleService.GetArticlesAsync(query, userId: User.GetOptionalUserId());
        return HandleResult(result);
    }

    [HttpGet("feed")]
    /// <summary>
    /// Returns a paginated list of articles from authors
    /// the user is following, optionally filtered by the author, tags or its favorited status for the user
    /// </summary>
    /// <param name="query">Parameters for filtering and pagination</param>
    public async Task<ActionResult> Feed([FromQuery] ArticleQueryParameters query)
    {
        var result = await _articleService.GetArticlesAsync(query, isFeed: true, userId: User.GetRequiredUserId());
        return HandleResult(result);
    }

    [AllowAnonymous]
    [HttpGet("{slug}")]
    /// <summary>
    /// Returns an article based on the slug
    /// </summary>
    /// <param name="slug">Article slug</param>
    public async Task<ActionResult> GetArticle(string slug)
    {
        var result = await _articleService.GetArticleBySlugAsync(slug, User.GetRequiredUserId());
        return HandleResult(result);
    }

    [HttpPost("")]
    /// <summary>
    /// Creates a new article
    /// </summary>
    public async Task<ActionResult> CreateArticle(CreateArticleRequest request)
    {
        var result = await _articleService.CreateAsync(request.article, User.GetRequiredUserId());
        return HandleResult(result);
    }

    [HttpPut("{slug}")]
    /// <summary>
    /// Updates an existing article
    /// </summary>
    public async Task<ActionResult> UpdateArticle(string slug, UpdateArticleRequest request)
    {
        var result = await _articleService.UpdateAsync(slug, request.article, User.GetRequiredUserId());
        return HandleResult(result);
    }

    [HttpDelete("{slug}")]
    /// <summary>
    /// Deletes an article
    /// </summary>
    public async Task<ActionResult> DeleteArticle(string slug)
    {
        var result = await _articleService.DeleteAsync(slug, User.GetRequiredUserId());
        return HandleResult(result);
    }

    [HttpPost("{slug}/favorite")]
    /// <summary>
    /// Favorites an article
    /// </summary>
    public async Task<ActionResult> FavoriteArticle(string slug)
    {
        var result = await _articleService.FavoriteArticleAsync(slug, User.GetRequiredUserId());
        return HandleResult(result);
    }

    [HttpDelete("{slug}/favorite")]
    /// <summary>
    /// Unfavorites an article
    /// </summary>
    public async Task<ActionResult> UnfavoriteArticle(string slug)
    {
        var result = await _articleService.UnfavoriteArticleAsync(slug, User.GetRequiredUserId());
        return HandleResult(result);
    }
}