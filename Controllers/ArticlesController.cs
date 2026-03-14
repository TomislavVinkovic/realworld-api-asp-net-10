using dotnet_api_tutorial.Data;
using dotnet_api_tutorial.DTOs;
using dotnet_api_tutorial.Models;
using dotnet_api_tutorial.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private IArticleService _articleService;

        public ArticlesController(
            AppDbContext context,
            IArticleService articleService
        )
        {
            _context = context;
            _articleService = articleService;
        }

        [HttpGet("")]
        public async Task<ActionResult> List([FromQuery] ArticleQueryParameters query)
        {
            
            var (articles, articleCount) = await _articleService.GetArticlesAsync(query);

            return Ok(
                new
                {
                    articles,
                    articleCount
                }
            );
        }

        [Authorize]
        [HttpGet("feed")]
        public async Task<ActionResult> Feed([FromQuery] ArticleQueryParameters query)
        {
            var (articles, articleCount) = await _articleService.GetArticlesAsync(query, isFeed: true);

            return Ok(
                new
                {
                    articles,
                    articleCount
                }
            );
        }

        [Authorize]
        [HttpGet("{slug}")]
        public async Task<ActionResult> GetArticle(string slug)
        {
            var article = _articleService.GetArticleBySlug(slug);
            if(article == null)
            {
                return NotFound();
            }

            return Ok(new {article});
        }

        [Authorize]
        [HttpPost("")]
        public async Task<ActionResult> CreateArticle(CreateArticleRequest request)
        {
            
        }
    }
}