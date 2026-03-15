using System.Security.Claims;
using dotnet_api_tutorial.Models;

namespace dotnet_api_tutorial.Services.Interface;

public interface IJwtService
{
    public string GenerateAccessToken(User user);
    public string GenerateRefreshToken();
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}