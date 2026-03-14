using dotnet_api_tutorial.DTOs;
using dotnet_api_tutorial.Models;

namespace dotnet_api_tutorial.Services.Interface;

public interface IArticleService {
    public Task<(IEnumerable<Article> articles, int Count)> GetArticlesAsync(
        ArticleQueryParameters query, 
        bool isFeed = false
    );
    public Task<Article?> GetArticleBySlugAsync(string slug);
    
    public Task<Article> CreateAsync(CreateArticleDto dto);
    public Task<Article?> UpdateAsync(string slug, UpdateArticleDto dto);
    public Task<bool> DeleteAsync(string slug);
}