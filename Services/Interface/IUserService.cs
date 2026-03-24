using RealWorld.DTOs;
using RealWorld.DTOs.Auth;
using RealWorld.Models;

namespace RealWorld.Services.Interface;

public interface IUserService
{
    Task<UserResponse?> LoginAsync(LoginDto dto);
    Task<bool> LogoutAsync();
    Task<UserResponse> RegisterAsync(RegisterDto dto);
    Task<UserResponse?> RefreshAsync(TokenRequest request);
    Task<UserResponse?> GetCurrentUserAsync(string currentToken);
    Task<UserResponse?> UpdateUserAsync(UpdateUserDto dto);
}