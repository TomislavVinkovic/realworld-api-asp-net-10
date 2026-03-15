using dotnet_api_tutorial.Data;
using dotnet_api_tutorial.DTOs;
using dotnet_api_tutorial.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace dotnet_api_tutorial.Services;

public class ProfileService : IProfileService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextService _httpContextService;

    public ProfileService(
        AppDbContext context,
        IHttpContextService httpContextService
    )
    {
        _context = context;
        _httpContextService = httpContextService;
    }

    public async Task<ProfileDto?> GetProfileByUsernameAsync(string username)
    {
        int? currentUserId = _httpContextService.GetCurrentUserId();
        if(currentUserId == null)
        {
            return null;
        }
        var currentUser = await _context.Users.FindAsync(currentUserId);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if(user == null)
        {
            return null;
        }

        bool isFollowingUser = currentUser.Following.Any(u => u.Username == user.Username);
        return new ProfileDto
        (
            user.Username,
            user.Bio,
            user.Image,
            isFollowingUser
        );
    }

    public async Task<ProfileDto?> FollowUserAsync(string username)
    {
        int? currentUserId = _httpContextService.GetCurrentUserId();
        if(currentUserId == null)
        {
            return null;
        }

        var currentUser = await _context.Users.FindAsync(currentUserId);
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

        return new ProfileDto
        (
            userToFollow.Username,
            userToFollow.Bio,
            userToFollow.Image,
            true
        );
    }

    public async Task<ProfileDto?> UnfollowUserAsync(string username)
    {
        int? currentUserId = _httpContextService.GetCurrentUserId();
        if(currentUserId == null)
        {
            return null;
        }

        var currentUser = await _context.Users.FindAsync(currentUserId);
        var userToFollow = await _context.Users
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();

        if(userToFollow == null)
        {
            return null;
        }

        if(currentUser!.Following.Any(u => u.Username == userToFollow.Username))
        {
            currentUser.Following.Remove(userToFollow);
            await _context.SaveChangesAsync();
        }

        return new ProfileDto
        (
            userToFollow.Username,
            userToFollow.Bio,
            userToFollow.Image,
            false
        );
    }
}