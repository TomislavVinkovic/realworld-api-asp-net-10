using System.Security.Claims;
using dotnet_api_tutorial.Models;

namespace dotnet_api_tutorial.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}