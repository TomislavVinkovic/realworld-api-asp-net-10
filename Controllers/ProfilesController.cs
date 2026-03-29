using RealWorld.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.Models.DTOs.Profiles;
using RealWorld.Extensions;

namespace RealWorld.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProfilesController : ApiControllerBase
{
    private IProfileService _profileService;

    public ProfilesController
    (
        IProfileService profileService
    )
    {
        _profileService = profileService;
    }
    
    /// <summary>
    /// Returns a profile based on the username
    /// </summary>
    /// <param name="username">User's username</param>
    [AllowAnonymous]
    [HttpGet("{username}")]
    public async Task<ActionResult<ProfileResponse>> GetProfile(string username)
    {
        var result = await _profileService.GetProfileByUsernameAsync(username, User.GetOptionalUserId());
        return HandleResult(result);
    }

    /// <summary>
    /// Follows a profile
    /// </summary>
    /// <param name="username">Username of the user the logged in user is trying to follow</param>
    [HttpPost("{username}/follow")]
    public async Task<ActionResult<ProfileResponse>> Follow(string username)
    {
        var result = await _profileService.FollowUserAsync(username, User.GetRequiredUserId());
        return HandleResult(result);
    }

    /// <summary>
    /// Unfollows a profile
    /// </summary>
    /// <param name="username">Username of the user the logged in user is trying to unfollow</param>
    [HttpDelete("{username}/follow")]
    public async Task<ActionResult<ProfileResponse>> Unfollow(string username)
    {
        var result = await _profileService.UnfollowUserAsync(username, User.GetRequiredUserId());
        return HandleResult(result);
    }

}
