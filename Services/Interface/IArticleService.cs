using RealWorld.Common;
using RealWorld.Models.DTOs;
using RealWorld.Models.DTOs.Articles;
using RealWorld.Models.Entities;

namespace RealWorld.Services.Interface;

public interface IArticleService {
    public Task<ServiceResult<ArticleListResponse>> GetArticlesAsync(
        ArticleQueryParameters query, 
        bool isFeed = false
    );
    public Task<ServiceResult<ArticleResponse?>> GetArticleBySlugAsync(string slug);
    
    public Task<ServiceResult<ArticleResponse>> CreateAsync(CreateArticleDto dto);
    public Task<ServiceResult<ArticleResponse?>> UpdateAsync(string slug, UpdateArticleDto dto);
    public Task<ServiceResult<bool>> DeleteAsync(string slug);

    public Task<ServiceResult<ArticleResponse?>> FavoriteArticleAsync(string slug);
    public Task<ServiceResult<ArticleResponse?>> UnfavoriteArticleAsync(string slug);

}