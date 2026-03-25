using RealWorld.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.Models.DTOs.Profiles;

namespace RealWorld.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProfilesController : ControllerBase
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
        var profile = await _profileService.GetProfileByUsernameAsync(username);
        if(profile == null)
        {
            return NotFound();
        }

        return Ok(
            new ProfileResponse(profile)
        );
    }

    [HttpPost("{username}/follow")]
    public async Task<ActionResult<ProfileResponse>> Follow(string username)
    {
        var profile = await _profileService.FollowUserAsync(username);
        if(profile == null)
        {
            return NotFound();
        }

        return Ok(
            new ProfileResponse(profile)
        );
    }

    [HttpDelete("{username}/unfollow")]
    public async Task<ActionResult<ProfileResponse>> Unfollow(string username)
    {
        var profile = await _profileService.UnfollowUserAsync(username);
        if(profile == null)
        {
            return NotFound();
        }

        return Ok(
            new ProfileResponse(profile)
        );
    }

}
