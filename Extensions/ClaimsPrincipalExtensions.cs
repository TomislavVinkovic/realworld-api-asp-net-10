using System.Security.Claims;

namespace RealWorld.Extensions;

public static class ClaimsPrincipalExtensions
{
    // For [Authorize] endpoints (Throws if missing)
    public static int GetRequiredUserId(this ClaimsPrincipal user)
    {
        var id = user.GetOptionalUserId();
        if (id == null)
        {
            throw new UnauthorizedAccessException("User ID not found.");
        }
        return id.Value;
    }

    // For [AllowAnonymous] endpoints (Returns null if missing)
    public static int? GetOptionalUserId(this ClaimsPrincipal user)
    {
        // If the user isn't logged in, ClaimsPrincipal is empty.
        var claim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("id");
        
        if (claim != null && int.TryParse(claim.Value, out int userId))
        {
            return userId;
        }

        return null;
    }
}