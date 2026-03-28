using RealWorld.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.Models.DTOs.Profiles;

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
    
    [AllowAnonymous]
    [HttpGet("{username}")]
    public async Task<ActionResult<ProfileResponse>> GetProfile(string username)
    {
        var result = await _profileService.GetProfileByUsernameAsync(username);
        return HandleResult(result);
    }

    [HttpPost("{username}/follow")]
    public async Task<ActionResult<ProfileResponse>> Follow(string username)
    {
        var result = await _profileService.FollowUserAsync(username);
        return HandleResult(result);
    }

    [HttpDelete("{username}/follow")]
    public async Task<ActionResult<ProfileResponse>> Unfollow(string username)
    {
        var result = await _profileService.UnfollowUserAsync(username);
        return HandleResult(result);
    }

}
