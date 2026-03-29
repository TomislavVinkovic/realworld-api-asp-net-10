using RealWorld.Common;
using RealWorld.Models.DTOs.Auth;

namespace RealWorld.Services.Interface;

public interface IUserService
{
    Task<ServiceResult<UserResponse?>> LoginAsync(LoginDto dto);
    Task<ServiceResult<bool>> LogoutAsync(int userId, string rawRefreshToken);
    Task<ServiceResult<UserResponse>> RegisterAsync(RegisterDto dto);
    Task<ServiceResult<UserResponse?>> RefreshAsync(TokenRequest request);
    Task<ServiceResult<UserResponse?>> GetCurrentUserAsync(string currentToken, int userId);
    Task<ServiceResult<UserResponse?>> UpdateUserAsync(UpdateUserDto dto, int userId);
}