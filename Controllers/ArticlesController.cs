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

    /// <summary>
    /// Returns a paginated list of articles, optionally filtered by the author, tags or its favorited status for the user
    /// </summary>
    /// <param name="tag">Tag</param>
    /// <param name="author">Author name</param>
    /// <param name="favorited">Show only articles favorited by the user</param>
    /// <param name="limit">How many articles to show per page</param>
    /// <param name="offset">Together with the limit controls the page the author is on</param>
    [AllowAnonymous]
    [HttpGet("")]
    public async Task<ActionResult> List([FromQuery] ArticleQueryParameters query)
    {
        var result = await _articleService.GetArticlesAsync(query, userId: User.GetOptionalUserId());
        return HandleResult(result);
    }

    /// <summary>
    /// Returns a paginated list of articles from authors
    /// the user is following, optionally filtered by the author, tags or its favorited status for the user
    /// </summary>
    /// <param name="tag">Tag</param>
    /// <param name="author">Author name</param>
    /// <param name="favorited">Show only articles favorited by the user</param>
    /// <param name="limit">How many articles to show per page</param>
    /// <param name="offset">Together with the limit controls the page the author is on</param>
    [HttpGet("feed")]
    public async Task<ActionResult> Feed([FromQuery] ArticleQueryParameters query)
    {
        var result = await _articleService.GetArticlesAsync(query, isFeed: true, userId: User.GetRequiredUserId());
        return HandleResult(result);
    }

    /// <summary>
    /// Returns an article based on the slug
    /// </summary>
    /// <param name="slug">Article slug</param>
    [AllowAnonymous]
    [HttpGet("{slug}")]
    public async Task<ActionResult> GetArticle(string slug)
    {
        var result = await _articleService.GetArticleBySlugAsync(slug, User.GetRequiredUserId());
        return HandleResult(result);
    }

    /// <summary>
    /// Creates a new article
    /// </summary>
    [HttpPost("")]
    public async Task<ActionResult> CreateArticle(CreateArticleRequest request)
    {
        var result = await _articleService.CreateAsync(request.article, User.GetRequiredUserId());
        return HandleResult(result);
    }

    /// <summary>
    /// Updates an existing article
    /// </summary>
    [HttpPut("{slug}")]
    public async Task<ActionResult> UpdateArticle(string slug, UpdateArticleRequest request)
    {
        var result = await _articleService.UpdateAsync(slug, request.article, User.GetRequiredUserId());
        return HandleResult(result);
    }

    /// <summary>
    /// Deletes an article
    /// </summary>
    [HttpDelete("{slug}")]
    public async Task<ActionResult> DeleteArticle(string slug)
    {
        var result = await _articleService.DeleteAsync(slug, User.GetRequiredUserId());
        return HandleResult(result);
    }

    /// <summary>
    /// Favorites an article
    /// </summary>
    [HttpPost("{slug}/favorite")]
    public async Task<ActionResult> FavoriteArticle(string slug)
    {
        var result = await _articleService.FavoriteArticleAsync(slug, User.GetRequiredUserId());
        return HandleResult(result);
    }

    /// <summary>
    /// Unfavorites an article
    /// </summary>
    [HttpDelete("{slug}/favorite")]
    public async Task<ActionResult> UnfavoriteArticle(string slug)
    {
        var result = await _articleService.UnfavoriteArticleAsync(slug, User.GetRequiredUserId());
        return HandleResult(result);
    }
}