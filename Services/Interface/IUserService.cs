using dotnet_api_tutorial.DTOs;

namespace dotnet_api_tutorial.Services.Interface;

public interface IUserService
{
    Task<UserResponse?> LoginAsync(LoginDto dto);
    Task<UserResponse> RegisterAsync(RegisterDto dto);
    Task<UserResponse?> RefreshAsync(TokenRequest request);
    Task<UserResponse?> GetCurrentUserAsync(string currentToken);
    Task<UserResponse?> UpdateUserAsync(UpdateUserDto dto);
}