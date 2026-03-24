using RealWorld.Models;

namespace RealWorld.DTOs.Articles;

public class AuthorDto
{
    public AuthorDto(User user, bool following)
    {
        Username = user.Username;
        Bio = user.Bio;
        Image = user.Image;
        Following = following;
    }
    public string Username {get; set;}
    public string? Bio {get; set;} = "";
    public string? Image {get; set;} = "";
    public bool Following {get; set;}
}