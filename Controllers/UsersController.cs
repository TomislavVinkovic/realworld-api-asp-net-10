using RealWorld.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.Models.DTOs.Auth;
using RealWorld.Extensions;
using Mapster;

namespace RealWorld.Controllers;

[Route("api/user")]
[ApiController]
public class UsersController : ApiControllerBase
{
    private readonly IUserService _userService;
    private readonly IFileService _fileService;

    public UsersController
    (
        IUserService userService,
        IFileService fileService
    )
    {
        _userService = userService;
        _fileService = fileService;
    }

    /// <summary>
    /// User login
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var result = await _userService.LoginAsync(request.user);
        return HandleResult(result);
    }

    /// <summary>
    /// User registration
    /// </summary>
    [HttpPost("")]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        var result = await _userService.RegisterAsync(request.user);
        return HandleResult(result);
    }

    /// <summary>
    /// The endpoint for refreshing jwt tokens
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh(TokenRequest request)
    {
        var result = await _userService.RefreshAsync(request);
        return HandleResult(result);
    }

    /// <summary>
    /// Logs out the user
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout([FromBody] TokenRequest request)
    {
        var result = await _userService.LogoutAsync(User.GetRequiredUserId(), request.RefreshToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Fetches the current user
    /// </summary>
    [Authorize]
    [HttpGet("")]
    public async Task<ActionResult> GetCurrentUser()
    {
        var currentAccessToken = HttpContext.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last() ?? "";

        var result = await _userService.GetCurrentUserAsync(currentAccessToken, User.GetRequiredUserId());
        return HandleResult(result);
    }

    /// <summary>
    /// Fetches the current user's data for editing
    /// </summary>
    [Authorize]
    [HttpGet("edit")]
    public async Task<ActionResult> EditUser()
    {
        var currentAccessToken = HttpContext.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last() ?? "";

        var result = await _userService.GetCurrentUserAsync(currentAccessToken, User.GetRequiredUserId());
        return HandleResult(result);
    }

    /// <summary>
    /// Updates the user
    /// </summary>
    [Authorize]
    [HttpPut("")]
    public async Task<ActionResult> UpdateUser ([FromForm] UpdateUserRequest request)
    {
        string? relativeImagePath = null;
        var file = request.user.Image;

        // Profile image upload
        if (file != null && file.Length > 0)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            using var stream = file.OpenReadStream();
            relativeImagePath = await _fileService.UploadAsync(stream, extension);
        }

        var updateDto = request.user.Adapt<UpdateUserDto>();
        updateDto.Image = relativeImagePath;

        var result = await _userService.UpdateUserAsync(updateDto, User.GetRequiredUserId());
        return HandleResult(result);
    }
}