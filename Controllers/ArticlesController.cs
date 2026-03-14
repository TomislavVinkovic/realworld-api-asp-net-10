using dotnet_api_tutorial.Data;
using dotnet_api_tutorial.DTOs;
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

        public ArticlesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<ActionResult> List([FromQuery] ArticleQueryParameters query)
        {
            var articlesQuery = _context.Articles
                .Include(a => a.Author)
                .Include(a => a.TagList)
                .AsQueryable();
            
            if(!string.IsNullOrWhiteSpace(query.Author))
            {
                articlesQuery = articlesQuery.Where(a => a.Author.Username == query.Author);
            }
            if(!string.IsNullOrWhiteSpace(query.Tag))
            {
                articlesQuery = articlesQuery.Where(a => a.TagList.Any(t => t.TagText == query.Tag));
            }
            if (!string.IsNullOrWhiteSpace(query.Favorited))
            {
                articlesQuery = articlesQuery.Where(a => a.FavoritedBy.Any(f => f.Username == query.Favorited));
            }

            // First DB query: Count the total number of articles
            var totalCount = await articlesQuery.CountAsync();
            var articles = await articlesQuery
                .OrderByDescending(a => a.CreatedAt)
                .Skip(query.Offset)
                .Take(query.Limit)
                .ToListAsync();

            return Ok(
                new
                {
                    articles = articles,
                    articleCount = totalCount
                }
            );
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Feed([FromQuery] ArticleQueryParameters query)
        {
            throw new NotImplementedException();
        }

    }
}