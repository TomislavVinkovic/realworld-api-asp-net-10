using dotnet_api_tutorial.DTOs;
using dotnet_api_tutorial.Models;

namespace dotnet_api_tutorial.Services.Interface;

public interface IArticleService {
    public Task<(IEnumerable<Article> articles, int Count)> GetArticlesAsync(
        ArticleQueryParameters query, 
        int? currentUserId = null, 
        bool isFeed = false
    ); 
}