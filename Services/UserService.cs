using System.Security.Claims;
using RealWorld.Data;
using RealWorld.DTOs;
using RealWorld.Models;
using RealWorld.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace RealWorld.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IFileService _fileService;
    private readonly IHttpContextService _httpContextService;

    public UserService(
        AppDbContext context, 
        IJwtService jwtService,
        IFileService fileService,
        IHttpContextService httpContextService
    )
    {
        _context = context;
        _jwtService = jwtService;
        _fileService = fileService;
        _httpContextService = httpContextService;
    }

    public async Task<UserResponse?> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            return null; // Signals to the controller to return 401 Unauthorized

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _context.SaveChangesAsync();

        var fullImageUrl = _fileService.GetAbsoluteFileUrl(user.Image);
        return new UserResponse(
            user.Email, 
            accessToken, 
            refreshToken, 
            user.Username, 
            user.Bio, 
            fullImageUrl
        );
    }

    public async Task<UserResponse> RegisterAsync(RegisterDto dto)
    {   
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var fullImageUrl = _fileService.GetAbsoluteFileUrl(user.Image);

        return new UserResponse(
            user.Email, 
            accessToken, 
            refreshToken, 
            user.Username, 
            user.Bio, 
            fullImageUrl
        );
    }

    public async Task<bool> LogoutAsync()
    {
        var userId = _httpContextService.GetCurrentUserId();
        if (userId == null) return false;

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        // 3. SECURE LOGOUT: Wipe the refresh token so it can never be used again
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<UserResponse?> RefreshAsync(TokenRequest request)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        var userIdString = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdString)) return null;

        var user = await _context.Users.FindAsync(int.Parse(userIdString));
        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null; // Signals invalid refresh token
        }

        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _context.SaveChangesAsync();

        var fullImageUrl = _fileService.GetAbsoluteFileUrl(user.Image);
        return new UserResponse(
            user.Email, 
            newAccessToken, 
            newRefreshToken, 
            user.Username, 
            user.Bio, 
            fullImageUrl
        );
    }

    public async Task<UserResponse?> GetCurrentUserAsync(string currentToken)
    {
        var userId = _httpContextService.GetCurrentUserId();
        if (userId == null) return null;

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        var fullImageUrl = _fileService.GetAbsoluteFileUrl(user.Image);
        return new UserResponse(
            user.Email, 
            currentToken, 
            user.RefreshToken!, 
            user.Username, 
            user.Bio, 
            fullImageUrl
        );
    }

    public async Task<UserResponse?> UpdateUserAsync(UpdateUserDto dto)
    {
        var userId = _httpContextService.GetCurrentUserId();
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null) return null;

        if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new ArgumentException("email"); // Caught by controller
            
            user.Email = dto.Email;
        }

        if (!string.IsNullOrEmpty(dto.Username) && dto.Username != user.Username)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                throw new ArgumentException("username"); // Caught by controller
            
            user.Username = dto.Username;
        }

        if (!string.IsNullOrEmpty(dto.Password))
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        if (dto.Bio != null) user.Bio = dto.Bio;
        if (dto.Image != null) user.Image = dto.Image;

        await _context.SaveChangesAsync();

        var newAccessToken = _jwtService.GenerateAccessToken(user);

        var fullImageUrl = _fileService.GetAbsoluteFileUrl(user.Image);
        return new UserResponse(
            user.Email, 
            newAccessToken, 
            user.RefreshToken, 
            user.Username, 
            user.Bio, 
            fullImageUrl
        );
    }
}