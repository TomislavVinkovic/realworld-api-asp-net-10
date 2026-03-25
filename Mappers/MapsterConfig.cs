using Mapster;
using RealWorld.DTOs.Articles;
using RealWorld.DTOs.Comments;
using RealWorld.Models;

namespace RealWorld.Mappings;

public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<Article, ArticleDto>.NewConfig()
            .Map(dest => dest.TagList, src => src.TagList.Select(t => t.TagText).ToArray())
            .Map(dest => dest.FavoritesCount, src => src.FavoritedBy.Count)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
            // Map the nested Author object using AuthorDto's configuration
            .Map(dest => dest.Author, src => src.Author.Adapt<AuthorDto>());
        
        TypeAdapterConfig<Comment, CommentDto>.NewConfig()
            .Map(dest => dest.CreatedAt, src => src.CreatedAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
            .Map(dest => dest.Author, src => src.Author.Adapt<AuthorDto>());
    }
}