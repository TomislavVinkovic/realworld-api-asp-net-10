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
        private readonly AppDbContext _context;
        private IProfileService _profileService;

        public ProfilesController(
            AppDbContext context,
            IProfileService profileService
        )
        {
            _context = context;
            _profileService = profileService;
        }
        
        [Authorize]
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
        [HttpPost("{username}")]
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
        [HttpDelete("{username}")]
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
