using RealWorld.Data;
using RealWorld.Models.Entities;
using RealWorld.Services.Interface;
using Microsoft.EntityFrameworkCore;
using RealWorld.Models.DTOs.Profiles;
using Mapster;
using RealWorld.Common;

namespace RealWorld.Services;

public class ProfileService : IProfileService
{
    private readonly AppDbContext _context;
    private readonly IFileService _fileService;

    public ProfileService(
        AppDbContext context,
        IHttpContextService httpContextService,
        IFileService fileService
    )
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<ServiceResult<ProfileResponse?>> GetProfileByUsernameAsync(string username, int? userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if(user == null)
        {
            return ServiceResult<ProfileResponse?>.NotFound($"User with ${username} not found.");
        }

        bool isFollowingUser = false;
        if(userId != null)
        {
            var currentUser = await _context.Users
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Id == userId);
            isFollowingUser = currentUser!.Following.Any(u => u.Username == user.Username);
        }
        
        var profileDto = ProfileDtoFactory(user, isFollowingUser);
        var response = new ProfileResponse(profileDto);

        return ServiceResult<ProfileResponse?>.Ok(response);
    }

    public async Task<ServiceResult<ProfileResponse?>> FollowUserAsync(string username, int userId)
    {
        var currentUser = await _context.Users
            .Include(u => u.Following)
            .FirstOrDefaultAsync(u => u.Id == userId);

        var userToFollow = await _context.Users
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();

        if(userToFollow == null)
        {
            return ServiceResult<ProfileResponse?>.NotFound($"User with ${username} not found.");
        }

        if(!currentUser!.Following.Any(u => u.Username == userToFollow.Username))
        {
            currentUser.Following.Add(userToFollow);
            await _context.SaveChangesAsync();
        }

        var profileDto = ProfileDtoFactory(userToFollow, true);
        var response = new ProfileResponse(profileDto);

        return ServiceResult<ProfileResponse?>.Ok(response);
    }

    public async Task<ServiceResult<ProfileResponse?>> UnfollowUserAsync(string username, int userId)
    {
        var currentUser = await _context.Users
            .Include(u => u.Following)
            .FirstOrDefaultAsync(u => u.Id == userId);
        var userToUnfollow = await _context.Users
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();

        if(userToUnfollow == null)
        {
            return ServiceResult<ProfileResponse?>.NotFound($"User with ${username} not found.");
        }

        if(currentUser!.Following.Any(u => u.Username == userToUnfollow.Username))
        {
            currentUser.Following.Remove(userToUnfollow);
            await _context.SaveChangesAsync();
        }

        var profileDto = ProfileDtoFactory(userToUnfollow, false);
        var response = new ProfileResponse(profileDto);

        return ServiceResult<ProfileResponse?>.Ok(response);
    }

    private ProfileDto ProfileDtoFactory(User user, bool isFollowingUser)
    {
        var profile = user.Adapt<ProfileDto>();
        var profileImageUrl = _fileService.GetAbsoluteFileUrl(user.Image);

        profile.Image = profileImageUrl;
        profile.Following = isFollowingUser;

        return profile;
    }
}