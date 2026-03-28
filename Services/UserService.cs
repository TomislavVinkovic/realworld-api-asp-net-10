using System.Security.Claims;
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

    public async Task<ServiceResult<UserResponse?>> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
        {
            return ServiceResult<UserResponse?>.Unauthorized("Invalid credentials");
        }

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _context.SaveChangesAsync();
        
        var userDto = await UserDtoFactory(user, accessToken, refreshToken);
        var response = new UserResponse(userDto);

        return ServiceResult<UserResponse?>.Ok(response);
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
        await _context.SaveChangesAsync();

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var userDto = await UserDtoFactory(user, accessToken, refreshToken);
        var response = new UserResponse(userDto);
        return ServiceResult<UserResponse>.Ok(response);
    }

    public async Task<ServiceResult<bool>> LogoutAsync()
    {
        var userId = _httpContextService.GetCurrentUserId();

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return ServiceResult<bool>.Unauthorized();
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    // To continue from here
    public async Task<ServiceResult<UserResponse?>> RefreshAsync(TokenRequest request)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        var userIdString = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdString))
        {
            return ServiceResult<UserResponse?>.BadRequest("Invalid or expired refresh token. Please log in again.");
        }

        var user = await _context.Users.FindAsync(int.Parse(userIdString));
        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return ServiceResult<UserResponse?>.BadRequest("Invalid or expired refresh token. Please log in again.");
        }

        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _context.SaveChangesAsync();

        var userDto = await UserDtoFactory(user, newAccessToken, newRefreshToken);
        var response = new UserResponse(userDto);

        return ServiceResult<UserResponse?>.Ok(response);
    }

    public async Task<ServiceResult<UserResponse?>> GetCurrentUserAsync(string currentToken)
    {
        var userId = _httpContextService.GetCurrentUserId();
        if (userId == null)
        {
            return ServiceResult<UserResponse?>.NotFound();
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return ServiceResult<UserResponse?>.NotFound();
        }

        var userDto = await UserDtoFactory(user, currentToken);
        var response = new UserResponse(userDto);

        return ServiceResult<UserResponse?>.Ok(response);
    }

    public async Task<ServiceResult<UserResponse?>> UpdateUserAsync(UpdateUserFormDto dto)
    {
        var userId = _httpContextService.GetCurrentUserId();
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null)
        {
            return ServiceResult<UserResponse?>.NotFound();
        }

        if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return ServiceResult<UserResponse?>.Fail("email has already been taken");
            }
            
            user.Email = dto.Email;
        }

        if (!string.IsNullOrEmpty(dto.Username) && dto.Username != user.Username)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                return ServiceResult<UserResponse?>.Fail("email has already been taken");
            }
            user.Username = dto.Username;
        }

        if (!string.IsNullOrEmpty(dto.Password))
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        if (dto.Bio != null)
        {
            user.Bio = dto.Bio;
        }

        ServiceResult<string> uploadResult;
        if (dto.Image != null && dto.Image.Length > 0)
        {
            uploadResult = await _fileService.UploadImageAsync(dto.Image);
            if(!uploadResult.Success)
            {
                return ServiceResult<UserResponse?>.Fail(uploadResult.Error!);
            }
        }

        await _context.SaveChangesAsync();

        var newAccessToken = _jwtService.GenerateAccessToken(user);

        var userDto = await UserDtoFactory(user, newAccessToken);
        var response = new UserResponse(userDto);

        return ServiceResult<UserResponse?>.Ok(response);
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
        userDto.RefreshToken = refreshToken == "" ? user.RefreshToken! : refreshToken;
        userDto.Image = fullImageUrl;

        return userDto;
    }
}