using RealWorld.Data;
using RealWorld.Models.DTOs;
using RealWorld.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.Models.DTOs.Articles;

namespace RealWorld.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ArticlesController : ControllerBase
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
        var (articles, articlesCount) = await _articleService.GetArticlesAsync(query);
        return Ok(new ArticleListResponse(articles, articlesCount));
    }

    [HttpGet("feed")]
    public async Task<ActionResult> Feed([FromQuery] ArticleQueryParameters query)
    {
        var (articles, articlesCount) = await _articleService.GetArticlesAsync(query, isFeed: true);
        return Ok(new ArticleListResponse(articles, articlesCount));
    }

    [AllowAnonymous]
    [HttpGet("{slug}")]
    public async Task<ActionResult> GetArticle(string slug)
    {
        var article = await _articleService.GetArticleBySlugAsync(slug);
        if(article == null)
        {
            return NotFound();
        }

        return Ok(new ArticleResponse(article));
    }

    [HttpPost("")]
    public async Task<ActionResult> CreateArticle(CreateArticleRequest request)
    {
        var article = await _articleService.CreateAsync(request.article);
        return Ok(new ArticleResponse(article));
    }

    [HttpPut("{slug}")]
    public async Task<ActionResult> UpdateArticle(string slug, UpdateArticleRequest request)
    {
        var article = await _articleService.UpdateAsync(slug, request.article);
        if(article == null)
        {
            return NotFound();
        }
        return Ok(new ArticleResponse(article));
    }

    [HttpDelete("{slug}")]
    public async Task<ActionResult> DeleteArticle(string slug)
    {
        bool deleted = await _articleService.DeleteAsync(slug);
        if(!deleted)
        {
            return NotFound();
        }
        return Ok();
    }

    [HttpPost("{slug}/favorite")]
    public async Task<ActionResult> FavoriteArticle(string slug)
    {
        var article = await _articleService.FavoriteArticleAsync(slug);
        if(article == null)
        {
            return NotFound();
        }
        return Ok(new ArticleResponse(article));
    }

    [HttpDelete("{slug}/unfavorite")]
    public async Task<ActionResult> UnfavoriteArticle(string slug)
    {
        var article = await _articleService.UnfavoriteArticleAsync(slug);
        if(article == null)
        {
            return NotFound();
        }
        return Ok(new ArticleResponse(article));
    }
}