using RealWorld.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.Models.DTOs.Articles;
using RealWorld.Extensions;
using RealWorld.Models.Entities;

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
    public async Task<ActionResult> List([FromQuery] ArticleQueryParameters query)
    {
        var result = await _articleService.GetArticlesAsync(query, userId: User.GetOptionalUserId());
        return HandleResult(result);
    }

    [HttpGet("feed")]
    public async Task<ActionResult> Feed([FromQuery] ArticleQueryParameters query)
    {
        var result = await _articleService.GetArticlesAsync(query, isFeed: true, userId: User.GetRequiredUserId());
        return HandleResult(result);
    }

    [AllowAnonymous]
    [HttpGet("{slug}")]
    public async Task<ActionResult> GetArticle(string slug)
    {
        var result = await _articleService.GetArticleBySlugAsync(slug, User.GetRequiredUserId());
        return HandleResult(result);
    }

    [HttpPost("")]
    public async Task<ActionResult> CreateArticle(CreateArticleRequest request)
    {
        var result = await _articleService.CreateAsync(request.article, User.GetRequiredUserId());
        return HandleResult(result);
    }

    [HttpPut("{slug}")]
    public async Task<ActionResult> UpdateArticle(string slug, UpdateArticleRequest request)
    {
        var result = await _articleService.UpdateAsync(slug, request.article, User.GetRequiredUserId());
        return HandleResult(result);
    }

    [HttpDelete("{slug}")]
    public async Task<ActionResult> DeleteArticle(string slug)
    {
        var result = await _articleService.DeleteAsync(slug, User.GetRequiredUserId());
        return HandleResult(result);
    }

    [HttpPost("{slug}/favorite")]
    public async Task<ActionResult> FavoriteArticle(string slug)
    {
        var result = await _articleService.FavoriteArticleAsync(slug, User.GetRequiredUserId());
        return HandleResult(result);
    }

    [HttpDelete("{slug}/favorite")]
    public async Task<ActionResult> UnfavoriteArticle(string slug)
    {
        var result = await _articleService.UnfavoriteArticleAsync(slug, User.GetRequiredUserId());
        return HandleResult(result);
    }
}