using RealWorld.Data;
using RealWorld.Models.Entities;
using RealWorld.Services.Interface;
using Microsoft.EntityFrameworkCore;
using RealWorld.Models.DTOs.Profiles;
using Mapster;

namespace RealWorld.Services;

public class ProfileService : IProfileService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextService _httpContextService;
    private readonly IFileService _fileService;

    public ProfileService(
        AppDbContext context,
        IHttpContextService httpContextService,
        IFileService fileService
    )
    {
        _context = context;
        _httpContextService = httpContextService;
        _fileService = fileService;
    }

    public async Task<ProfileDto?> GetProfileByUsernameAsync(string username)
    {
        int? currentUserId = _httpContextService.GetCurrentUserId();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if(user == null)
        {
            return null;
        }

        bool isFollowingUser = false;
        if(currentUserId != null)
        {
            var currentUser = await _context.Users
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);
            isFollowingUser = currentUser!.Following.Any(u => u.Username == user.Username);
        }

        return ProfileDtoFactory(user, isFollowingUser);
    }

    public async Task<ProfileDto?> FollowUserAsync(string username)
    {
        int? currentUserId = _httpContextService.GetCurrentUserId();
        if(currentUserId == null)
        {
            return null;
        }

        var currentUser = await _context.Users
            .Include(u => u.Following)
            .FirstOrDefaultAsync(u => u.Id == currentUserId);

        var userToFollow = await _context.Users
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();

        if(userToFollow == null)
        {
            return null;
        }

        if(!currentUser!.Following.Any(u => u.Username == userToFollow.Username))
        {
            currentUser.Following.Add(userToFollow);
            await _context.SaveChangesAsync();
        }
        return ProfileDtoFactory(userToFollow, true);
    }

    public async Task<ProfileDto?> UnfollowUserAsync(string username)
    {
        int? currentUserId = _httpContextService.GetCurrentUserId();
        if(currentUserId == null)
        {
            return null;
        }

        var currentUser = await _context.Users
            .Include(u => u.Following)
            .FirstOrDefaultAsync(u => u.Id == currentUserId);
        var userToUnfollow = await _context.Users
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();

        if(userToUnfollow == null)
        {
            return null;
        }

        if(currentUser!.Following.Any(u => u.Username == userToUnfollow.Username))
        {
            currentUser.Following.Remove(userToUnfollow);
            await _context.SaveChangesAsync();
        }

        return ProfileDtoFactory(userToUnfollow, false);
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