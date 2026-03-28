using RealWorld.Common;
using RealWorld.Models.DTOs.Profiles;

namespace RealWorld.Services.Interface;

public interface IProfileService
{
    public Task<ServiceResult<ProfileResponse?>> GetProfileByUsernameAsync(string username, int? userId);
    public Task<ServiceResult<ProfileResponse?>> FollowUserAsync(string username, int userId);
    public Task<ServiceResult<ProfileResponse?>> UnfollowUserAsync(string username, int userId);
}