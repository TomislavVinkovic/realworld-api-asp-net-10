using Mapster;
using RealWorld.Models.DTOs.Articles;
using RealWorld.Models.DTOs.Comments;
using RealWorld.Models.DTOs.Profiles;
using RealWorld.Models.Entities;

namespace RealWorld.Mappings;

public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<User, AuthorDto>.NewConfig()
            .Ignore(dest => dest.Following);
        
        TypeAdapterConfig<User, ProfileDto>.NewConfig()
            .Ignore(dest => dest.Following);

        TypeAdapterConfig<Article, ArticleDto>.NewConfig()
            .Ignore(dest => dest.Favorited)
            .Map(
                dest => dest.TagList, 
                src => src.TagList.Select(t => t.TagText).OrderBy(t => t).ToList()
            )
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