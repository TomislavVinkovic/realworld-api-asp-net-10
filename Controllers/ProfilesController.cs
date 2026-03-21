using dotnet_api_tutorial.Data;
using dotnet_api_tutorial.DTOs;
using dotnet_api_tutorial.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_api_tutorial.Controllers
{
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

        [Authorize]
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

        [Authorize]
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
}
