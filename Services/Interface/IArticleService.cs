using RealWorld.DTOs;
using RealWorld.Models;

namespace RealWorld.Services.Interface;

public interface IArticleService {
    public Task<(IEnumerable<ArticleDto> articles, int Count)> GetArticlesAsync(
        ArticleQueryParameters query, 
        bool isFeed = false
    );
    public Task<ArticleDto?> GetArticleBySlugAsync(string slug);
    
    public Task<ArticleDto> CreateAsync(CreateArticleDto dto);
    public Task<ArticleDto?> UpdateAsync(string slug, UpdateArticleDto dto);
    public Task<bool> DeleteAsync(string slug);

    public Task<ArticleDto?> FavoriteArticleAsync(string slug);
    public Task<ArticleDto?> UnfavoriteArticleAsync(string slug);

}