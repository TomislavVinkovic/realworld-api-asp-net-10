using RealWorld.Models.DTOs;
using RealWorld.Models.DTOs.Profiles;

namespace RealWorld.Services.Interface;

public interface IProfileService
{
    public Task<ProfileDto?> GetProfileByUsernameAsync(string username);
    public Task<ProfileDto?> FollowUserAsync(string username);
    public Task<ProfileDto?> UnfollowUserAsync(string username);
}