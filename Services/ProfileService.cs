using RealWorld.Data;
using RealWorld.DTOs;
using RealWorld.Models;
using RealWorld.Services.Interface;
using Microsoft.EntityFrameworkCore;

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
        
        var profileImageUrl = _fileService.GetAbsoluteFileUrl(user.Image);
        return new ProfileDto
        (
            Username: user.Username,
            Bio: user.Bio,
            Image: profileImageUrl,
            Following: isFollowingUser
        );
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