using System.Security.Claims;
using RealWorld.Models;

namespace RealWorld.Services.Interface;

public interface IJwtService
{
    public string GenerateAccessToken(User user);
    public string GenerateRefreshToken();
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}