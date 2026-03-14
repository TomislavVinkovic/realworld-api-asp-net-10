using dotnet_api_tutorial.DTOs;
using dotnet_api_tutorial.Models;
using dotnet_api_tutorial.Services.Interface;

namespace dotnet_api_tutorial.Services;


public class ArticleService : IArticleService {
    public async Task<(IEnumerable<Article> articles, int Count)> GetArticlesAsync(
        ArticleQueryParameters query, 
        int? currentUserId = null, 
        bool isFeed = false
    )
    {
        throw new NotImplementedException();
    }
}