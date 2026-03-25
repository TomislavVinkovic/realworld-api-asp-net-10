using RealWorld.Models.DTOs;
using RealWorld.Models.DTOs.Auth;
using RealWorld.Models.Entities;

namespace RealWorld.Services.Interface;

public interface IUserService
{
    Task<UserDto?> LoginAsync(LoginDto dto);
    Task<bool> LogoutAsync();
    Task<UserDto> RegisterAsync(RegisterDto dto);
    Task<UserDto?> RefreshAsync(TokenRequest request);
    Task<UserDto?> GetCurrentUserAsync(string currentToken);
    Task<UserDto?> UpdateUserAsync(UpdateUserDto dto);
}