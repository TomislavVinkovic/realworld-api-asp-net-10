using RealWorld.Data;
using RealWorld.DTOs;
using RealWorld.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RealWorld.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        [HttpGet("")]
        public async Task<ActionResult> List([FromQuery] ArticleQueryParameters query)
        {
            var (articles, articlesCount) = await _articleService.GetArticlesAsync(query);
            return Ok(new ArticleListResponse(articles, articlesCount));
        }

        [Authorize]
        [HttpGet("feed")]
        public async Task<ActionResult> Feed([FromQuery] ArticleQueryParameters query)
        {
            var (articles, articlesCount) = await _articleService.GetArticlesAsync(query, isFeed: true);
            return Ok(new ArticleListResponse(articles, articlesCount));
        }

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

        [Authorize]
        [HttpPost("")]
        public async Task<ActionResult> CreateArticle(CreateArticleRequest request)
        {
            try
            {
                var article = await _articleService.CreateAsync(request.article);
                return Ok(new ArticleResponse(article));
            }
            catch(UnauthorizedAccessException _)
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpPut("{slug}")]
        public async Task<ActionResult> UpdateArticle(string slug, UpdateArticleRequest request)
        {
            try
            {
                var article = await _articleService.UpdateAsync(slug, request.article);
                if(article == null)
                {
                    return NotFound();
                }
                return Ok(new ArticleResponse(article));
            }
            catch(UnauthorizedAccessException _)
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpDelete("{slug}")]
        public async Task<ActionResult> DeleteArticle(string slug)
        {
            try
            {
                bool deleted = await _articleService.DeleteAsync(slug);
                if(!deleted)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch (UnauthorizedAccessException _)
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpPost("{slug}/favorite")]
        public async Task<ActionResult> FavoriteArticle(string slug)
        {
            try
            {
                var article = await _articleService.FavoriteArticleAsync(slug);
                if(article == null)
                {
                    return NotFound();
                }
                return Ok(new ArticleResponse(article));
            }
            catch (UnauthorizedAccessException _)
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpDelete("{slug}/unfavorite")]
        public async Task<ActionResult> UnfavoriteArticle(string slug)
        {
            try
            {
                var article = await _articleService.UnfavoriteArticleAsync(slug);
                if(article == null)
                {
                    return NotFound();
                }
                return Ok(new ArticleResponse(article));
            }
            catch (UnauthorizedAccessException _)
            {
                return Unauthorized();
            }
        }
    }
}