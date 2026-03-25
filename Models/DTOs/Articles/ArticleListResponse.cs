namespace RealWorld.Models.DTOs.Articles;

public record ArticleListResponse(IEnumerable<ArticleDto> articles, int articlesCount);