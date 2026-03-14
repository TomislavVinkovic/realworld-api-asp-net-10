using System.Security.Claims;
using dotnet_api_tutorial.Data;
using dotnet_api_tutorial.DTOs;
using dotnet_api_tutorial.Models;
using dotnet_api_tutorial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_api_tutorial.Controllers;

    
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;

    // Inject our new service alongside the database context
    public UsersController(AppDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponse>> Login(LoginRequest request)
    {
        var userData = request.user;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userData.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(userData.Password, user.Password))
            return Unauthorized("Invalid credentials.");

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _context.SaveChangesAsync();

        return Ok(
            new
            {
                user = new UserResponse(
                    user.Email,
                    accessToken,
                    refreshToken,
                    user.Username,
                    user.Bio,
                    user.Image
                )
            }
        );
    }

    [HttpPost("")]
    public async Task<ActionResult<UserResponse>> Register(RegisterRequest request)
    {
        var userData = request.user;

        if (await _context.Users.AnyAsync(u => u.Email == userData.Email))
            return BadRequest("Email already in use.");
        
        var user = new User
        {
            Username = userData.Username,
            Email = userData.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(userData.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        return Ok(
            new UserResponse(
                user.Email,
                accessToken,
                refreshToken,
                user.Username,
                user.Bio,
                user.Image
            )
        );
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<UserResponse>> Refresh(TokenRequest request)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        var userId = int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await _context.Users.FindAsync(userId);
        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return BadRequest("Invalid or expired refresh token. Please log in again.");
        }

        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _context.SaveChangesAsync();

        return Ok(
            new
            {
                user = new UserResponse(
                    user.Email,
                    newAccessToken,
                    newRefreshToken,
                    user.Username,
                    user.Bio,
                    user.Image
                )
            }
        );
    }

    [Authorize]
    [HttpGet("")]
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null) return NotFound();

        var currentAccessToken = HttpContext.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last() ?? "";

        return Ok(
            new {
                user = 
                    new UserResponse(
                        Email: user.Email, 
                        Token: currentAccessToken,
                        RefreshToken: user.RefreshToken!, 
                        Username: user.Username, 
                        Bio: user.Bio, 
                        Image: user.Image
                    )
            }
        );
    }

    [Authorize]
    [HttpPut("")]
    public async Task<ActionResult> UpdateUser(UpdateUserRequest request)
    {
        var userData = request.user;
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null) return NotFound();

        var dto = request;

        if (!string.IsNullOrEmpty(userData.Email) && userData.Email != user.Email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == userData.Email))
                return BadRequest(new { errors = new { email = new[] { "has already been taken" } } });
            
            user.Email = userData.Email;
        }

        if (!string.IsNullOrEmpty(userData.Username) && userData.Username != user.Username)
        {
            if (await _context.Users.AnyAsync(u => u.Username == userData.Username))
                return BadRequest(new { errors = new { username = new[] { "has already been taken" } } });
            
            user.Username = userData.Username;
        }

        if (!string.IsNullOrEmpty(userData.Password))
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(userData.Password);
        }

        if (userData.Bio != null) user.Bio = userData.Bio;
        if (userData.Image != null) user.Image = userData.Image;

        await _context.SaveChangesAsync();

        var newAccessToken = _jwtService.GenerateAccessToken(user);

        var userPayload = new 
        {
            email = user.Email,
            token = newAccessToken,
            refreshToken = user.RefreshToken,
            username = user.Username,
            bio = user.Bio,
            image = user.Image
        };

        return Ok(new { user = userPayload });
    }
}