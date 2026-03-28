using RealWorld.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.Models.DTOs.Articles;

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
        var result = await _articleService.GetArticlesAsync(query);
        return HandleResult(result);
    }

    [HttpGet("feed")]
    public async Task<ActionResult> Feed([FromQuery] ArticleQueryParameters query)
    {
        var result = await _articleService.GetArticlesAsync(query, isFeed: true);
        return HandleResult(result);
    }

    [AllowAnonymous]
    [HttpGet("{slug}")]
    public async Task<ActionResult> GetArticle(string slug)
    {
        var result = await _articleService.GetArticleBySlugAsync(slug);
        return HandleResult(result);
    }

    [HttpPost("")]
    public async Task<ActionResult> CreateArticle(CreateArticleRequest request)
    {
        var result = await _articleService.CreateAsync(request.article);
        return HandleResult(result);
    }

    [HttpPut("{slug}")]
    public async Task<ActionResult> UpdateArticle(string slug, UpdateArticleRequest request)
    {
        var result = await _articleService.UpdateAsync(slug, request.article);
        return HandleResult(result);
    }

    [HttpDelete("{slug}")]
    public async Task<ActionResult> DeleteArticle(string slug)
    {
        var result = await _articleService.DeleteAsync(slug);
        return HandleResult(result);
    }

    [HttpPost("{slug}/favorite")]
    public async Task<ActionResult> FavoriteArticle(string slug)
    {
        var result = await _articleService.FavoriteArticleAsync(slug);
        return HandleResult(result);
    }

    [HttpDelete("{slug}/favorite")]
    public async Task<ActionResult> UnfavoriteArticle(string slug)
    {
        var result = await _articleService.UnfavoriteArticleAsync(slug);
        return HandleResult(result);
    }
}