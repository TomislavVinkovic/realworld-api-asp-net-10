using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using RealWorld.Data;
using RealWorld.Models.Entities;
using RealWorld.Services.Interface;
using Microsoft.EntityFrameworkCore;
using RealWorld.Models.DTOs.Auth;
using Mapster;
using RealWorld.Common;

namespace RealWorld.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IFileService _fileService;

    public UserService(
        AppDbContext context,
        IJwtService jwtService,
        IFileService fileService
    )
    {
        _context = context;
        _jwtService = jwtService;
        _fileService = fileService;
    }

    public async Task<ServiceResult<UserResponse?>> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            return ServiceResult<UserResponse?>.Unauthorized("Invalid credentials");

        var accessToken = _jwtService.GenerateAccessToken(user);
        var (rawRefreshToken, hashedRefreshToken) = GenerateRefreshToken();

        // Each login = new family (new device/session)
        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = hashedRefreshToken,
            Family = Guid.NewGuid().ToString(),
            ExpiryTime = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        var userDto = await UserDtoFactory(user, accessToken, rawRefreshToken);
        return ServiceResult<UserResponse?>.Ok(new UserResponse(userDto));
    }

    public async Task<ServiceResult<UserResponse>> RegisterAsync(RegisterDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(); // Save first to get user.Id

        var accessToken = _jwtService.GenerateAccessToken(user);
        var (rawRefreshToken, hashedRefreshToken) = GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = hashedRefreshToken,
            Family = Guid.NewGuid().ToString(),
            ExpiryTime = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        var userDto = await UserDtoFactory(user, accessToken, rawRefreshToken);
        return ServiceResult<UserResponse>.Ok(new UserResponse(userDto));
    }

    public async Task<ServiceResult<bool>> LogoutAsync(int userId, string rawRefreshToken)
    {
        var hashed = HashToken(rawRefreshToken);

        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Token == hashed);

        if (token == null)
            return ServiceResult<bool>.Unauthorized();

        // Only revoke this session, not all devices
        token.IsRevoked = true;
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<UserResponse?>> RefreshAsync(TokenRequest request)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        var userIdString = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdString))
            return ServiceResult<UserResponse?>.BadRequest("Invalid access token.");

        var hashed = HashToken(request.RefreshToken);
        var stored = await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == hashed);

        if (stored == null)
            return ServiceResult<UserResponse?>.BadRequest("Invalid refresh token.");

        // Reuse detected — token was already rotated, family is compromised
        if (stored.IsRevoked)
        {
            await RevokeFamilyAsync(stored.Family);
            return ServiceResult<UserResponse?>.BadRequest(
                "Token reuse detected. Please log in again."
            );
        }

        if (stored.ExpiryTime <= DateTime.UtcNow)
            return ServiceResult<UserResponse?>.BadRequest("Refresh token expired.");

        // Rotate — revoke old, issue new in same family
        stored.IsRevoked = true;

        var (newRawToken, newHashedToken) = GenerateRefreshToken();
        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = stored.UserId,
            Token = newHashedToken,
            Family = stored.Family, // same family, keeps chain trackable
            ExpiryTime = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        var newAccessToken = _jwtService.GenerateAccessToken(stored.User);
        var userDto = await UserDtoFactory(stored.User, newAccessToken, newRawToken);
        return ServiceResult<UserResponse?>.Ok(new UserResponse(userDto));
    }

    public async Task<ServiceResult<UserResponse?>> GetCurrentUserAsync(string currentToken, int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return ServiceResult<UserResponse?>.NotFound();

        var userDto = await UserDtoFactory(user, currentToken);
        return ServiceResult<UserResponse?>.Ok(new UserResponse(userDto));
    }

    public async Task<ServiceResult<UserResponse?>> UpdateUserAsync(UpdateUserDto dto, int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return ServiceResult<UserResponse?>.NotFound();

        if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return ServiceResult<UserResponse?>.Fail("Email has already been taken.");
            user.Email = dto.Email;
        }

        if (!string.IsNullOrEmpty(dto.Username) && dto.Username != user.Username)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return ServiceResult<UserResponse?>.Fail("Username has already been taken.");
            user.Username = dto.Username;
        }

        if (!string.IsNullOrEmpty(dto.Password))
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        if (dto.Bio != null) user.Bio = dto.Bio;
        if (dto.Image != null) user.Image = dto.Image;

        await _context.SaveChangesAsync();

        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var userDto = await UserDtoFactory(user, newAccessToken);
        return ServiceResult<UserResponse?>.Ok(new UserResponse(userDto));
    }

    // --- Private helpers ---

    private async Task RevokeFamilyAsync(string family)
    {
        var familyTokens = await _context.RefreshTokens
            .Where(t => t.Family == family && !t.IsRevoked)
            .ToListAsync();

        foreach (var t in familyTokens)
            t.IsRevoked = true;

        await _context.SaveChangesAsync();
    }

    private static (string raw, string hashed) GenerateRefreshToken()
    {
        var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        return (raw, HashToken(raw));
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }

    private async Task<UserDto> UserDtoFactory(
        User user,
        string accessToken,
        string refreshToken = ""
    )
    {
        var fullImageUrl = _fileService.GetAbsoluteFileUrl(user.Image);
        var userDto = user.Adapt<UserDto>();
        userDto.Token = accessToken;
        userDto.RefreshToken = refreshToken == "" ? string.Empty : refreshToken;
        userDto.Image = fullImageUrl;
        return userDto;
    }
}