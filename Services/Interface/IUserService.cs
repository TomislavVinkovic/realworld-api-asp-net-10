using RealWorld.Common;
using RealWorld.Models.DTOs;
using RealWorld.Models.DTOs.Auth;
using RealWorld.Models.Entities;

namespace RealWorld.Services.Interface;

public interface IUserService
{
    Task<ServiceResult<UserResponse?>> LoginAsync(LoginDto dto);
    Task<ServiceResult<bool>> LogoutAsync();
    Task<ServiceResult<UserResponse>> RegisterAsync(RegisterDto dto);
    Task<ServiceResult<UserResponse?>> RefreshAsync(TokenRequest request);
    Task<ServiceResult<UserResponse?>> GetCurrentUserAsync(string currentToken);
    Task<ServiceResult<UserResponse?>> UpdateUserAsync(UpdateUserFormDto dto);
}