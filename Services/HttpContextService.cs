using System.Security.Claims;
using RealWorld.Services.Interface;

namespace RealWorld.Services;

class HttpContextService : IHttpContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextService(
        IHttpContextAccessor httpContextAccessor
    )
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetBaseUrl()
    {
        var request = _httpContextAccessor.HttpContext!.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";

        return baseUrl;
    }

    public int? GetCurrentUserId()
    {
        var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(userIdString == null)
        {
            return null;
        }
        int currentUserId = int.Parse(userIdString);

        return currentUserId;
    }
}